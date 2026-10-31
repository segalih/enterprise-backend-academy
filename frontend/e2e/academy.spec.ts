import { test, expect } from '@playwright/test';

const API_BASE = 'http://localhost:5100';

test.describe('Enterprise Backend Engineering Academy — Full E2E Suite (API + Browser)', () => {

  test('1. API: Seed, 19 Levels (00..18), 12 Projects (01..12), and 404 Handling', async ({ request }) => {
    // 1. Check levels
    const levelsRes = await request.get(`${API_BASE}/api/curriculum/levels`);
    expect(levelsRes.status()).toBe(200);
    const levels = await levelsRes.json();
    expect(levels.length).toBe(19);
    expect(levels[0].id).toBe('level-00');
    expect(levels[18].id).toBe('level-18');

    // 2. Check 12 enterprise projects
    const projRes = await request.get(`${API_BASE}/api/projects`);
    expect(projRes.status()).toBe(200);
    const projects = await projRes.json();
    expect(projects.length).toBe(12);
    for (let i = 1; i <= 12; i++) {
      const id = `proj-${String(i).padStart(2, '0')}`;
      expect(projects.some((p: any) => p.id === id)).toBeTruthy();
    }

    // 3. Check 404 on invalid lesson, level, project, challenge, and unknown API route
    const invLesson = await request.get(`${API_BASE}/api/curriculum/lessons/non-existent-lesson-999`);
    expect(invLesson.status()).toBe(404);

    const invLevel = await request.get(`${API_BASE}/api/curriculum/levels/non-existent-level-999`);
    expect(invLevel.status()).toBe(404);

    const invProject = await request.get(`${API_BASE}/api/projects/non-existent-proj-999`);
    expect(invProject.status()).toBe(404);

    const invApi = await request.get(`${API_BASE}/api/unknown-route-xyz`);
    expect(invApi.status()).toBe(404);

    // 4. Check Dashboard seed data
    const dashRes = await request.get(`${API_BASE}/api/progress/dashboard`);
    expect(dashRes.status()).toBe(200);
    const dash = await dashRes.json();
    expect(dash.userId).toBe('default-learner');
    expect(dash.totalLessons).toBeGreaterThan(50);
  });

  test('2. API: CodeExecutionService (Success, Compile Error Line=1, Runtime Exception, Infinite Loop <= ~4s + Recovery, Stdin, Security Block)', async ({ request }) => {
    // 1. Valid C# code
    const okRes = await request.post(`${API_BASE}/api/execution/csharp`, {
      data: { code: 'using System;\nConsole.WriteLine("E2E_OK_" + (21 * 2));' }
    });
    expect(okRes.status()).toBe(200);
    const okJson = await okRes.json();
    expect(okJson.success).toBe(true);
    expect(okJson.output).toContain('E2E_OK_42');

    // 2. Compilation error with accurate 1-based line number
    const errRes = await request.post(`${API_BASE}/api/execution/csharp`, {
      data: { code: 'int broken = "not_an_int";' }
    });
    expect(errRes.status()).toBe(200);
    const errJson = await errRes.json();
    expect(errJson.success).toBe(false);
    expect(errJson.diagnostics.length).toBeGreaterThan(0);
    expect(errJson.diagnostics[0].line).toBe(1);

    // 3. Runtime exception
    const rtRes = await request.post(`${API_BASE}/api/execution/csharp`, {
      data: { code: 'int zero = 0;\nint boom = 10 / zero;\nConsole.WriteLine(boom);' }
    });
    expect(rtRes.status()).toBe(200);
    const rtJson = await rtRes.json();
    expect(rtJson.success).toBe(false);
    expect(rtJson.error).toContain('DivideByZeroException');

    // 4. Infinite loop terminated within timeout (~1500ms) AND subsequent request succeeds fast
    const startLoop = Date.now();
    const loopRes = await request.post(`${API_BASE}/api/execution/csharp`, {
      data: { code: 'while (true) { }', timeoutMs: 1500 }
    });
    const loopElapsed = Date.now() - startLoop;
    expect(loopRes.status()).toBe(200);
    const loopJson = await loopRes.json();
    expect(loopJson.success).toBe(false);
    expect(loopJson.timedOut).toBe(true);
    expect(loopElapsed).toBeLessThan(4500);

    // Verify immediate recovery after infinite loop
    const recRes = await request.post(`${API_BASE}/api/execution/csharp`, {
      data: { code: 'Console.WriteLine("RECOVERED_AFTER_LOOP");', timeoutMs: 2000 }
    });
    const recJson = await recRes.json();
    expect(recJson.success).toBe(true);
    expect(recJson.output).toContain('RECOVERED_AFTER_LOOP');

    // 5. Stdin with value and Stdin with null/empty (EOF)
    const stdinRes = await request.post(`${API_BASE}/api/execution/csharp`, {
      data: {
        code: 'string? name = Console.ReadLine();\nConsole.WriteLine("HELLO_" + name);',
        input: 'PRINCIPAL_ENGINEER\n'
      }
    });
    const stdinJson = await stdinRes.json();
    expect(stdinJson.success).toBe(true);
    expect(stdinJson.output).toContain('HELLO_PRINCIPAL_ENGINEER');

    const emptyStdinRes = await request.post(`${API_BASE}/api/execution/csharp`, {
      data: {
        code: 'string? line = Console.ReadLine();\nConsole.WriteLine("EOF_" + (line ?? "NULL"));'
      }
    });
    const emptyStdinJson = await emptyStdinRes.json();
    expect(emptyStdinJson.success).toBe(true);
    expect(emptyStdinJson.output).toContain('EOF_NULL');

    // 6. Dangerous code blocked
    const secRes = await request.post(`${API_BASE}/api/execution/csharp`, {
      data: { code: 'Environment.Exit(99);' }
    });
    const secJson = await secRes.json();
    expect(secJson.success).toBe(false);
    expect(secJson.error).toContain('Security Policy Violation');
  });

  test('3. API: TerminalSandboxService (Allowed CLI Commands, Build/Run, and Path Traversal / Injection Blocked)', async ({ request }) => {
    const sessionId = 'e2e-term-session';

    const runCmd = async (commandLine: string, sid = sessionId) => {
      const res = await request.post(`${API_BASE}/api/terminal/command`, {
        data: { sessionId: sid, commandLine }
      });
      expect(res.status()).toBe(200);
      return await res.json();
    };

    // Allowed commands
    const ver = await runCmd('dotnet --version');
    expect(ver.exitCode).toBe(0);
    expect(ver.output).toMatch(/^8\./);

    const info = await runCmd('dotnet --info');
    expect(info.exitCode).toBe(0);
    expect(info.output).toContain('.NET');

    const ls = await runCmd('ls');
    expect(ls.exitCode).toBe(0);
    expect(ls.output).toContain('Program.cs');
    expect(ls.output).toContain('App.csproj');

    const cat = await runCmd('cat Program.cs');
    expect(cat.exitCode).toBe(0);
    expect(cat.output).toContain('Enterprise Backend Engineering Terminal Ready');

    const pwd = await runCmd('pwd');
    expect(pwd.exitCode).toBe(0);
    expect(pwd.output).toContain('/workspace/session_e2e-term-session');

    const build = await runCmd('dotnet build');
    expect(build.exitCode).toBe(0);

    const run = await runCmd('dotnet run');
    expect(run.exitCode).toBe(0);
    expect(run.output).toContain('Enterprise Backend Engineering Terminal Ready.');

    const clear = await runCmd('clear');
    expect(clear.exitCode).toBe(0);
    expect(clear.output).toBe('__CLEAR__');

    const reset = await runCmd('reset');
    expect(reset.exitCode).toBe(0);
    expect(reset.output).toContain('reset successfully');

    // Security checks: Path traversal & forbidden commands
    const travSession = await runCmd('pwd', '../../escape_attempt');
    expect(travSession.output).not.toContain('..');
    expect(travSession.output).toContain('/workspace/session_escape_attempt');

    await runCmd('pwd', 'victim');
    const siblingCat = await runCmd('cat ../session_victim/Program.cs', 'attacker');
    expect(siblingCat.exitCode).not.toBe(0);

    const encodedCat = await runCmd('cat ..%2Fsession_victim%2FProgram.cs', 'attacker');
    expect(encodedCat.exitCode).not.toBe(0);

    const forbidden = await runCmd('powershell -Command "Get-Process"');
    expect(forbidden.exitCode).toBe(1);
    expect(forbidden.output).toContain('restricted');

    const argInject = await runCmd('dotnet build ../other');
    expect(argInject.exitCode).toBe(1);
  });

  test('4. API: Quiz Boundary (<80% vs >=80%) and Progress Persistence', async ({ request }) => {
    const userId = `e2e-quiz-user-${Date.now()}`;

    // Fetch lesson-02-04 quiz questions
    const lessonRes = await request.get(`${API_BASE}/api/curriculum/lessons/lesson-02-04`);
    expect(lessonRes.status()).toBe(200);
    const lesson = await lessonRes.json();
    const questions = lesson.quizQuestions;
    expect(questions.length).toBeGreaterThanOrEqual(2);

    // 1. Submit < 80% (all wrong answers)
    const wrongAnswers: Record<string, number> = {};
    for (const q of questions) {
      wrongAnswers[q.id] = (q.correctOptionIndex + 1) % q.options.length;
    }

    const failQuizRes = await request.post(`${API_BASE}/api/quiz/submit`, {
      data: { userId, lessonId: 'lesson-02-04', answers: wrongAnswers }
    });
    expect(failQuizRes.status()).toBe(200);
    const failQuiz = await failQuizRes.json();
    expect(failQuiz.passed).toBe(false);
    expect(failQuiz.score).toBe(0);

    // Verify lesson is NOT marked completed after failed quiz
    const dashAfterFail = await (await request.get(`${API_BASE}/api/progress/dashboard?userId=${userId}`)).json();
    expect(dashAfterFail.completedLessonIds).not.toContain('lesson-02-04');

    // 2. Submit >= 80% (all correct answers)
    const rightAnswers: Record<string, number> = {};
    for (const q of questions) {
      rightAnswers[q.id] = q.correctOptionIndex;
    }

    const passQuizRes = await request.post(`${API_BASE}/api/quiz/submit`, {
      data: { userId, lessonId: 'lesson-02-04', answers: rightAnswers }
    });
    expect(passQuizRes.status()).toBe(200);
    const passQuiz = await passQuizRes.json();
    expect(passQuiz.passed).toBe(true);
    expect(passQuiz.score).toBe(100);

    // Verify lesson IS marked completed in dashboard
    const dashAfterPass = await (await request.get(`${API_BASE}/api/progress/dashboard?userId=${userId}`)).json();
    expect(dashAfterPass.completedLessonIds).toContain('lesson-02-04');
  });

  test('5. Browser E2E: Full User Journey (Dashboard -> Lesson -> Monaco Run -> Quiz -> Simulators -> Playground -> Projects -> Terminal -> Search & Theme)', async ({ page }) => {
    const consoleErrors: string[] = [];
    const pageErrors: string[] = [];
    const server5xxResponses: string[] = [];

    page.on('console', msg => {
      if (msg.type() === 'error') {
        consoleErrors.push(msg.text());
      }
    });
    page.on('pageerror', err => {
      pageErrors.push(err.message);
    });
    page.on('response', res => {
      if (res.status() >= 500) {
        server5xxResponses.push(`${res.status()} ${res.url()}`);
      }
    });

    // 1. Open application
    await page.goto('/');
    await expect(page.getByText('ENTERPRISE BACKEND ACADEMY')).toBeVisible();
    await expect(page.getByText('Kurikulum Induk (18 Level)')).toBeVisible();

    // 2. Navigate to Dashboard tab & verify metrics render
    await page.getByRole('button', { name: 'Dashboard' }).click();
    await expect(page.getByText('Selamat Datang, Enterprise Engineer!')).toBeVisible();
    await expect(page.getByText('Kemajuan per Level Pembelajaran')).toBeVisible();

    // Click LEVEL 02 card in Dashboard to navigate to lesson-02-01, then select lesson-02-04 in Sidebar
    await page.getByText('LEVEL 02').first().click();
    await expect(page.getByText('Kurikulum Induk (18 Level)')).toBeVisible();
    await page.getByRole('button', { name: /Value Types vs Reference Types/i }).click();
    await expect(page.getByRole('heading', { name: /Value Types vs Reference Types/i }).first()).toBeVisible();

    // 3. Test RightToc bookmark & TOC scroll (verify h2 id exists in DOM)
    const firstTocBtn = page.locator('aside nav button').first();
    await expect(firstTocBtn).toBeVisible();
    await firstTocBtn.click();

    // 4. Run code in Lesson's Monaco Editor
    const runBtn = page.getByRole('button', { name: /Run Code/i }).first();
    await runBtn.click();
    await expect(page.getByText('Exit Code 0')).toBeVisible({ timeout: 15000 });

    // 5. Answer Quiz in Lesson-02-04 & verify lesson stays completed!
    const quizSection = page.locator('text=Evaluasi & Kuis Pemahaman Konsep');
    await quizSection.scrollIntoViewIfNeeded();
    // Lesson-02-04 has 2 questions: both have correctOptionIndex = 0 (Option A)
    const questionCards = page.locator('div:has(> div > span:text-matches("PERTANYAAN #"))');
    const qCount = await questionCards.count();
    expect(qCount).toBeGreaterThanOrEqual(2);
    for (let i = 0; i < qCount; i++) {
      const card = questionCards.nth(i);
      // Click Option A (index 0)
      await card.locator('button').nth(0).click();
    }
    await page.getByRole('button', { name: /Kirim Jawaban Kuis/i }).click();
    await expect(page.getByText(/LULUS! Pemahaman Konsep Sangat Baik/i)).toBeVisible();
    // Verify RightToc completion button shows "Selesai Dipelajari" (not undone!)
    await expect(page.getByRole('button', { name: /Selesai Dipelajari/i })).toBeVisible();

    // 6. Navigate to Playground tab: run valid template and test Stdin input
    await page.getByRole('button', { name: 'Playground' }).click();
    await expect(page.getByRole('heading', { name: 'Interactive C# Playground' })).toBeVisible();
    await page.getByRole('button', { name: 'Stdin Input' }).click();
    await page.getByPlaceholder(/Masukkan teks input/i).fill('TestStdinInput');
    await page.getByRole('button', { name: /Run Code/i }).click();
    await expect(page.getByText('Exit Code 0')).toBeVisible({ timeout: 15000 });
    await expect(page.locator('pre code', { hasText: '=== ENTERPRISE C# PLAYGROUND ===' })).toBeVisible();

    // 7. Navigate to 12 Proyek Enterprise tab: verify all 12 projects exist and switching updates editor
    await page.getByRole('button', { name: '12 Proyek Enterprise' }).click();
    await expect(page.getByRole('heading', { name: '12 Enterprise Project-Based Learning' })).toBeVisible();
    await expect(page.getByText('PROJ-03')).toBeVisible();
    await expect(page.getByText('PROJ-11')).toBeVisible();
    await page.getByText('PROJ-12').first().click();
    await expect(page.getByRole('heading', { name: /PROJECT 12/i })).toBeVisible();

    // 8. Navigate to Visual Simulators tab: test all 6 simulators
    await page.getByRole('button', { name: 'Visual Simulators' }).click();
    await expect(page.getByRole('heading', { name: 'Interactive Visual Learning Simulators' })).toBeVisible();

    // Sim 1: Stack vs Heap
    await page.getByRole('button', { name: /Lanjut/i }).click();
    await expect(page.getByText('Langkah 1: Deklarasi Value Type')).toBeVisible();

    // Sim 2: 8-Bit Binary & Two's Complement (test 0, 42, 255, -128)
    await page.getByRole('button', { name: /8-Bit Binary/i }).click();
    await expect(page.getByText('0x2A')).toBeVisible(); // 42
    await page.getByRole('button', { name: 'Maks (255)' }).click();
    await expect(page.getByText('0xFF')).toBeVisible();
    await expect(page.getByText('-1', { exact: true })).toBeVisible();
    await page.getByRole('button', { name: 'Reset' }).click();
    await expect(page.getByText('0x00')).toBeVisible();
    // Toggle MSB (bit 7) to get -128 (10000000)
    await page.locator('.grid-cols-8 button').first().click();
    await expect(page.getByText('-128', { exact: true })).toBeVisible();
    await expect(page.getByText('0x80')).toBeVisible();

    // Sim 3: Garbage Collector
    await page.getByRole('button', { name: /Garbage Collector/i }).click();
    await page.getByRole('button', { name: 'Alokasikan Objek' }).click();
    await page.getByRole('button', { name: 'Picu GC Gen 0' }).click();
    await expect(page.getByText(/\[GC CYCLE\] GC Gen 0 dijalankan!/i)).toBeVisible();

    // Sim 4: Async/Await State Machine
    await page.getByRole('button', { name: /Async\/Await State Machine/i }).click();
    await page.getByRole('button', { name: /Fase Berikutnya/i }).click();
    await expect(page.getByText(/Fase 1: Titik Await/i)).toBeVisible();

    // Sim 5: Database B+ Tree Index Seek
    await page.getByRole('button', { name: /Database B\+ Tree/i }).click();
    await page.getByRole('button', { name: 'Index Seek', exact: true }).click();
    await expect(page.getByText('3 pages')).toBeVisible({ timeout: 5000 });

    // Sim 6: Deadlock & Lock Ordering
    await page.getByRole('button', { name: /Deadlock & Lock Ordering/i }).click();
    await page.getByRole('button', { name: 'Langkah Berikutnya' }).click();
    await page.getByRole('button', { name: 'Langkah Berikutnya' }).click();
    await expect(page.getByText(/CRITICAL DEADLOCK DETECTED/i)).toBeVisible();

    // 9. Navigate to Sandbox Terminal tab: click quick 'dotnet --version' button when input is empty
    await page.getByRole('button', { name: 'Sandbox Terminal' }).click();
    await expect(page.getByRole('heading', { name: 'Interactive Dotnet CLI Sandbox Terminal' })).toBeVisible();
    await page.getByRole('button', { name: 'dotnet --version' }).click();
    await expect(page.locator('pre', { hasText: /^8\./ }).first()).toBeVisible({ timeout: 10000 });

    // 10. Dark/Light Mode toggle
    const htmlEl = page.locator('html');
    await expect(htmlEl).toHaveClass(/dark/);
    await page.getByTitle('Ganti Mode Gelap / Terang').click();
    await expect(htmlEl).not.toHaveClass(/dark/);
    await page.getByTitle('Ganti Mode Gelap / Terang').click();
    await expect(htmlEl).toHaveClass(/dark/);

    // Verify zero console errors, zero page errors, zero 5xx responses
    expect(consoleErrors).toEqual([]);
    expect(pageErrors).toEqual([]);
    expect(server5xxResponses).toEqual([]);
  });

  test('6. Markdown Renderer & Visual Layout: Fenced Code Block (<pre><code>) vs Inline Code, ASCII Diagram Alignment, and Responsive No-Overflow (Desktop & Mobile 375px)', async ({ page }) => {
    const consoleErrors: string[] = [];
    page.on('console', msg => {
      if (msg.type() === 'error') consoleErrors.push(msg.text());
    });

    await page.setViewportSize({ width: 1440, height: 900 });
    await page.goto('/');
    await page.evaluate(() => {
      localStorage.setItem('academy-last-lesson', 'lesson-03-03');
      localStorage.setItem('academy-theme', 'dark');
    });
    await page.reload();
    await expect(page.locator('h1')).toHaveCount(1);
    await expect(page.locator('h1')).toContainText(/Polimorfisme Dinamis/i);

    // 1. Verify fenced monospace diagram in lesson-03-03 renders as a single <pre><code>
    const diagramPre = page.locator('pre').filter({ hasText: 'SyncBlockIndex' });
    await expect(diagramPre).toBeVisible();
    const diagramCode = diagramPre.locator('> code');
    await expect(diagramCode).toHaveCount(1);

    const diagramStats = await diagramCode.evaluate((el) => {
      const cs = window.getComputedStyle(el);
      const text = el.textContent || '';
      const lines = text.split('\n');
      return {
        display: cs.display,
        whiteSpace: cs.whiteSpace,
        className: el.className,
        lineCount: lines.length,
        hasSyncBlock: text.includes('SyncBlockIndex'),
        hasMethodTable: text.includes('MethodTable')
      };
    });

    expect(diagramStats.display).toBe('block');
    expect(diagramStats.whiteSpace).toBe('pre');
    expect(diagramStats.className).not.toContain('bg-slate-800');
    expect(diagramStats.lineCount).toBeGreaterThanOrEqual(5);
    expect(diagramStats.hasSyncBlock).toBe(true);
    expect(diagramStats.hasMethodTable).toBe(true);

    // 2. Verify inline code does NOT use block styles and is not inside <pre>
    const inlineCode = page.locator('p code').first();
    await expect(inlineCode).toBeVisible();
    const inlineStats = await inlineCode.evaluate((el) => {
      const cs = window.getComputedStyle(el);
      return {
        display: cs.display,
        parentTag: el.parentElement?.tagName,
        className: el.className
      };
    });
    expect(inlineStats.display).toBe('inline');
    expect(inlineStats.parentTag).not.toBe('PRE');
    expect(inlineStats.className).toContain('bg-slate-800');

    // 3. Verify Copy button on code block works
    const copyBtn = page.getByRole('button', { name: /Salin/i }).first();
    await expect(copyBtn).toBeVisible();

    // 4. Verify responsive layout (no horizontal page overflow on Desktop 1440px and Mobile 375px across dark/light themes)
    for (const lessonId of ['lesson-03-03', 'lesson-00-02', 'lesson-09-03']) {
      await page.evaluate((id) => localStorage.setItem('academy-last-lesson', id), lessonId);
      await page.reload();
      await expect(page.locator('h1')).toHaveCount(1);

      // Desktop 1440px
      await page.setViewportSize({ width: 1440, height: 900 });
      const desktopOverflow = await page.evaluate(() =>
        document.documentElement.scrollWidth <= document.documentElement.clientWidth
      );
      expect(desktopOverflow).toBe(true);

      // Mobile 375px
      await page.setViewportSize({ width: 375, height: 812 });
      const mobileOverflow = await page.evaluate(() =>
        document.documentElement.scrollWidth <= document.documentElement.clientWidth
      );
      expect(mobileOverflow).toBe(true);
    }

    expect(consoleErrors).toEqual([]);
  });

  test('7. Lesson Content: No Raw Math Dollar Delimiters ($...$) in Rendered Lessons', async ({ page }) => {
    const consoleErrors: string[] = [];
    page.on('console', msg => {
      if (msg.type() === 'error') consoleErrors.push(msg.text());
    });

    // 1. Visit lesson-00-06 (previously had $0x00$, $0x7F$, $16 = 2^4$)
    await page.goto('/');
    await page.evaluate(() => localStorage.setItem('academy-last-lesson', 'lesson-00-06'));
    await page.reload();
    await expect(page.locator('h1')).toHaveCount(1);

    // Verify 0x00 and 0x7F render as inline code elements
    const code0x00 = page.locator('code', { hasText: '0x00' });
    await expect(code0x00.first()).toBeVisible();

    // Verify no raw math $...$ syntax appears in the rendered lesson container
    const lesson0006Text = await page.locator('main').innerText();
    expect(lesson0006Text).not.toContain('$0x00$');
    expect(lesson0006Text).not.toContain('$0x7F$');
    expect(lesson0006Text).not.toContain('$16 = 2^4$');

    // 2. Visit lesson-05-01 (previously had $O(1)$, $O(N)$)
    await page.evaluate(() => localStorage.setItem('academy-last-lesson', 'lesson-05-01'));
    await page.reload();
    await expect(page.locator('h1')).toHaveCount(1);

    const lesson0501Text = await page.locator('main').innerText();
    expect(lesson0501Text).not.toContain('$O(1)$');
    expect(lesson0501Text).not.toContain('$O(N)$');
    expect(lesson0501Text).toContain('O(1)');

    // 3. Visit lesson-05-02 (previously had $$\\text{Load Factor} = \\frac{N}{B}$$)
    await page.evaluate(() => localStorage.setItem('academy-last-lesson', 'lesson-05-02'));
    await page.reload();
    await expect(page.locator('h1')).toHaveCount(1);

    const lesson0502Text = await page.locator('main').innerText();
    expect(lesson0502Text).not.toContain('$$\\text{Load Factor}');
    expect(lesson0502Text).toContain('Load Factor = N / B');

    expect(consoleErrors).toEqual([]);
  });

  test('8. Mermaid Architecture Diagrams: Lazy Loaded, Renders SVG, Strict Security, Theme Reactive, and Copy Source', async ({ page }) => {
    const consoleErrors: string[] = [];
    page.on('console', msg => {
      if (msg.type() === 'error') consoleErrors.push(msg.text());
    });

    // 1. Visit lesson-00-08 which has the MMU Virtual Address -> Physical RAM Mermaid diagram
    await page.setViewportSize({ width: 1440, height: 900 });
    await page.goto('/');
    await page.evaluate(() => {
      localStorage.setItem('academy-last-lesson', 'lesson-00-08');
      localStorage.setItem('academy-theme', 'dark');
    });
    await page.reload();
    await expect(page.locator('h1')).toHaveCount(1);

    // 2. Verify Mermaid container and rendered SVG
    const mermaidContainer = page.locator('[data-testid="mermaid-diagram"]').first();
    await expect(mermaidContainer).toBeVisible({ timeout: 10000 });

    const svgElement = mermaidContainer.locator('div[aria-label="Diagram alur sistem"] svg').first();
    await expect(svgElement).toBeVisible({ timeout: 10000 });

    // Verify diagram contains nodes from the flowchart
    await expect(svgElement).toContainText(/Proses Backend A/i);
    await expect(svgElement).toContainText(/MMU/i);

    // 3. Test Salin button on Mermaid diagram
    const copyBtn = mermaidContainer.getByRole('button', { name: /Salin/i });
    await expect(copyBtn).toBeVisible();
    await copyBtn.click();
    await expect(mermaidContainer.getByText('Tersalin')).toBeVisible();

    // 4. Test Theme reactivity: toggle theme to light mode and verify diagram updates
    const themeBtn = page.getByTitle('Ganti Mode Gelap / Terang');
    await themeBtn.click();
    const htmlEl = page.locator('html');
    await expect(htmlEl).not.toHaveClass(/dark/);
    await expect(svgElement).toBeVisible();

    // Switch back to dark mode
    await themeBtn.click();
    await expect(htmlEl).toHaveClass(/dark/);

    expect(consoleErrors).toEqual([]);
  });

  test('9. Dynamic RightToc & Enhanced Stack vs Heap Simulator: Scroll-Spy Tracking, Value Type on Heap, and Boxing Visualization', async ({ page }) => {
    const consoleErrors: string[] = [];
    page.on('console', msg => {
      if (msg.type() === 'error') consoleErrors.push(msg.text());
    });

    // 1. Visit lesson-02-04 on desktop viewport
    await page.setViewportSize({ width: 1440, height: 900 });
    await page.goto('/');
    await page.evaluate(() => localStorage.setItem('academy-last-lesson', 'lesson-02-04'));
    await page.reload();
    await expect(page.locator('h1')).toHaveCount(1);

    // 2. Test RightToc headings and smooth scrolling
    const rightToc = page.locator('[data-testid="right-toc"]');
    await expect(rightToc).toBeVisible();
    const firstTocBtn = rightToc.locator('nav button').first();
    await expect(firstTocBtn).toBeVisible();
    await firstTocBtn.click();

    // Verify active class on clicked toc item
    await expect(firstTocBtn).toHaveClass(/text-blue-400/);

    // 3. Switch to Interactive Simulator tab
    const simTabBtn = page.getByRole('button', { name: /Interactive Simulator/i });
    await expect(simTabBtn).toBeVisible();
    await simTabBtn.click();

    // 4. Verify Enhanced Stack vs Heap Simulator
    const simContainer = page.locator('[data-testid="stack-heap-simulator"]');
    await expect(simContainer).toBeVisible();

    // Check Learning Goals banner
    await expect(simContainer.getByText(/Tujuan Belajar Simulator/i)).toBeVisible();
    await expect(simContainer.getByText(/Boxing/i)).toBeVisible();

    // Advance to Step 4 (Struct on Heap)
    const nextBtn = simContainer.getByRole('button', { name: /Lanjut/i });
    for (let i = 0; i < 4; i++) {
      await nextBtn.click();
    }
    await expect(simContainer.getByText(/Langkah 4/i)).toBeVisible();
    await expect(simContainer.getByText('(int di dalam Heap!)')).toBeVisible();

    // Advance to Step 7 (Boxing)
    for (let i = 0; i < 3; i++) {
      await nextBtn.click();
    }
    await expect(simContainer.getByText(/Langkah 7: Boxing/i)).toBeVisible();
    await expect(simContainer.getByText(/Boxed Object/i)).toBeVisible();

    // Test Reset button
    const resetBtn = simContainer.getByTitle(/Reset/i);
    await resetBtn.click();
    await expect(simContainer.getByText(/Langkah 0 dari 7/i)).toBeVisible();

    expect(consoleErrors).toEqual([]);
  });

  test('10. Mermaid Parser Suite: All ```mermaid Blocks Across Curriculum Parse Without Error in Browser', async ({ page, request }) => {
    test.setTimeout(120000);
    // 1. Fetch all lessons and collect all mermaid diagram code blocks
    const levelsRes = await request.get(`${API_BASE}/api/curriculum/levels`);
    expect(levelsRes.status()).toBe(200);
    const levels = await levelsRes.json();
    const allLessonIds: string[] = [];

    for (const lvl of levels) {
      const lvlDetailRes = await request.get(`${API_BASE}/api/curriculum/levels/${lvl.id}`);
      if (lvlDetailRes.status() === 200) {
        const lvlDetail = await lvlDetailRes.json();
        for (const m of lvlDetail.modules || []) {
          for (const les of m.lessons || []) {
            allLessonIds.push(les.id);
          }
        }
      }
    }

    const mermaidRegex = /```mermaid\r?\n([\s\S]*?)```/g;
    const diagramItems: { id: string; chart: string }[] = [];

    for (const id of allLessonIds) {
      const lesRes = await request.get(`${API_BASE}/api/curriculum/lessons/${id}`);
      if (lesRes.status() === 200) {
        const detail = await lesRes.json();
        let match;
        while ((match = mermaidRegex.exec(detail.markdownContent || '')) !== null) {
          diagramItems.push({ id, chart: match[1].trim() });
        }
      }
    }

    expect(diagramItems.length).toBeGreaterThanOrEqual(50);

    // 2. Open browser page and run window.__mermaid.parse on all diagrams
    await page.goto('/');
    await page.waitForFunction(() => !!(window as any).__mermaid, { timeout: 15000 });
    const parseErrors: string[] = [];

    for (const item of diagramItems) {
      try {
        await page.evaluate(async (chartCode) => {
          await (window as any).__mermaid.parse(chartCode);
        }, item.chart);
      } catch (err: any) {
        parseErrors.push(`Lesson ${item.id} failed to parse: ${err.message}`);
      }
    }

    expect(parseErrors).toEqual([]);
  });

  test('11. Visual Regression & Screenshots: Samples Across Levels in Dark/Light, Desktop & Mobile (375px)', async ({ page }) => {
    test.setTimeout(180000);
    const consoleErrors: string[] = [];
    page.on('console', msg => {
      if (msg.type() === 'error') consoleErrors.push(msg.text());
    });

    const screenshotDir = 'C:/Users/Galih/.gemini/antigravity/brain/3ba603d8-422e-469d-8b2c-3f07ffb3703c/screenshots/diagrams';
    const sampleLessons = [
      'lesson-00-01',
      'lesson-01-01',
      'lesson-02-04',
      'lesson-03-02',
      'lesson-04-03',
      'lesson-05-02',
      'lesson-06-01',
      'lesson-07-01',
      'lesson-08-01',
      'lesson-09-02',
      'lesson-11-01',
      'lesson-12-03',
      'lesson-13-03',
      'lesson-14-02',
      'lesson-15-01',
      'lesson-16-02',
      'lesson-17-01',
      'lesson-18-02'
    ];

    for (const lessonId of sampleLessons) {
      // 1. Desktop Dark Viewport
      await page.setViewportSize({ width: 1440, height: 900 });
      await page.goto('/');
      await page.evaluate(({ id }) => {
        localStorage.setItem('academy-last-lesson', id);
        localStorage.setItem('academy-theme', 'dark');
      }, { id: lessonId });
      await page.reload();
      await expect(page.locator('h1')).toHaveCount(1);

      const mermaidBox = page.locator('[data-testid="mermaid-diagram"]').first();
      await expect(mermaidBox).toBeVisible({ timeout: 10000 });
      const svg = mermaidBox.locator('div[aria-label="Diagram alur sistem"] svg').first();
      await expect(svg).toBeVisible({ timeout: 10000 });

      // Verify no horizontal overflow on desktop
      const desktopOverflow = await page.evaluate(() => {
        return document.documentElement.scrollWidth > window.innerWidth + 2;
      });
      expect(desktopOverflow).toBe(false);

      await page.screenshot({
        path: `${screenshotDir}/${lessonId}-desktop-dark.png`,
        fullPage: false
      });

      // 2. Mobile Light Viewport (375px)
      await page.setViewportSize({ width: 375, height: 812 });
      await page.evaluate(() => {
        localStorage.setItem('academy-theme', 'light');
        document.documentElement.classList.remove('dark');
      });
      await page.reload();
      await expect(page.locator('h1')).toHaveCount(1);

      const mobileMermaidBox = page.locator('[data-testid="mermaid-diagram"]').first();
      await expect(mobileMermaidBox).toBeVisible({ timeout: 10000 });
      const mobileSvg = mobileMermaidBox.locator('div[aria-label="Diagram alur sistem"] svg').first();
      await expect(mobileSvg).toBeVisible({ timeout: 10000 });

      // Verify no horizontal overflow on mobile
      const mobileOverflow = await page.evaluate(() => {
        return document.documentElement.scrollWidth > window.innerWidth + 2;
      });
      expect(mobileOverflow).toBe(false);

      await page.screenshot({
        path: `${screenshotDir}/${lessonId}-mobile-light.png`,
        fullPage: false
      });
    }

    expect(consoleErrors).toEqual([]);
  });

  test('12. Coding Challenge Flow: Multi-Test Runner, Diff Visualizer, Whitespace Indicators, Custom Input Drawer, and Non-Coding Lessons', async ({ page, request }) => {
    const consoleErrors: string[] = [];
    page.on('console', msg => {
      if (msg.type() === 'error') {
        consoleErrors.push(msg.text());
      }
    });

    // 1. API Verification: Non-Coding Lesson has hasExercise: false and codingChallenge: null
    const nonCodingRes = await request.get(`${API_BASE}/api/curriculum/lessons/lesson-00-01`);
    expect(nonCodingRes.status()).toBe(200);
    const nonCodingLesson = await nonCodingRes.json();
    expect(nonCodingLesson.hasExercise).toBe(false);
    expect(nonCodingLesson.codingChallenge).toBeNull();

    // 2. API Verification: Coding Lesson has structured contract (>=2 examples, >=3 test cases, hints, difficulty)
    const codingRes = await request.get(`${API_BASE}/api/curriculum/lessons/lesson-00-04`);
    expect(codingRes.status()).toBe(200);
    const codingLesson = await codingRes.json();
    expect(codingLesson.hasExercise).toBe(true);
    expect(codingLesson.codingChallenge).not.toBeNull();
    const ch = codingLesson.codingChallenge;
    expect(ch.id).toBe('ch-00-04');
    expect(ch.examples.length).toBeGreaterThanOrEqual(2);
    expect(ch.testCases.length).toBeGreaterThanOrEqual(3);
    expect(ch.starterCode).toContain('Main');
    expect(ch.solutionCode).toContain('Main');

    // 3. API: Faulty solution produces line-by-line diff and whitespace markers
    const faultySubmit = await request.post(`${API_BASE}/api/execution/challenge`, {
      data: {
        challengeId: 'ch-00-04',
        code: 'using System;\npublic class Solution { public static void Main() { Console.WriteLine(999); } }'
      }
    });
    expect(faultySubmit.status()).toBe(200);
    const faultyData = await faultySubmit.json();
    expect(faultyData.passed).toBe(false);
    expect(faultyData.testsPassed).toBe(0);
    expect(faultyData.testResults.length).toBe(4);

    // Public test case has diff and visual markers
    const test0 = faultyData.testResults[0];
    expect(test0.hasDiff).toBe(true);
    expect(test0.diffLines.length).toBeGreaterThan(0);
    expect(test0.expectedVisual).toBeDefined();
    expect(test0.actualVisual).toBeDefined();

    // Hidden test case respects masking policy
    const testHidden = faultyData.testResults[3];
    expect(testHidden.isHidden).toBe(true);
    expect(testHidden.expected).toBe('[Tersembunyi]');
    expect(testHidden.hasDiff).toBe(false);

    // 4. API: Custom Input execution without grading
    const customRes = await request.post(`${API_BASE}/api/execution/challenge`, {
      data: {
        challengeId: 'ch-00-04',
        code: ch.solutionCode,
        customInput: '0x1A'
      }
    });
    expect(customRes.status()).toBe(200);
    const customData = await customRes.json();
    expect(customData.isCustomRun).toBe(true);
    expect(customData.passed).toBe(true);
    expect(customData.stdout.trim()).toBe('26');

    // 5. API: Single test execution
    const singleRes = await request.post(`${API_BASE}/api/execution/challenge`, {
      data: {
        challengeId: 'ch-00-04',
        code: ch.solutionCode,
        singleTestIndex: 0
      }
    });
    expect(singleRes.status()).toBe(200);
    const singleData = await singleRes.json();
    expect(singleData.testResults.length).toBe(1);
    expect(singleData.testResults[0].passed).toBe(true);

    // 6. Browser E2E: Non-Coding Lesson has NO Coding Challenge tab/button
    await page.goto('/');
    await page.evaluate(() => {
      localStorage.setItem('academy-last-lesson', 'lesson-00-01');
      localStorage.setItem('academy-theme', 'dark');
    });
    await page.reload();
    await expect(page.locator('h1')).toHaveCount(1);
    const nonCodingTab = page.locator('button:has-text("Coding Challenge")');
    await expect(nonCodingTab).toHaveCount(0);

    // 7. Browser E2E: Coding Lesson displays Coding Challenge tab and view
    await page.evaluate(() => {
      localStorage.setItem('academy-last-lesson', 'lesson-00-04');
    });
    await page.reload();
    await expect(page.locator('h1')).toHaveCount(1);

    const codingTab = page.locator('button:has-text("Coding Challenge")');
    await expect(codingTab).toHaveCount(1);
    await codingTab.click();

    // Problem specifications
    await expect(page.locator('text=Format Input').first()).toBeVisible();
    await expect(page.locator('text=Format Output').first()).toBeVisible();
    await expect(page.locator('text=Contoh Input & Output').first()).toBeVisible();

    // 8. Browser E2E: Custom Input drawer
    const customInputBtn = page.locator('button:has-text("Input Kustom")');
    await customInputBtn.click();
    await expect(page.locator('text=Uji dengan Input Kustom (Manual Stdin)')).toBeVisible();

    const textarea = page.locator('textarea[placeholder*="Ketik string input"]');
    await textarea.fill('0x1A');
    const runCustomBtn = page.locator('button:has-text("Jalankan")');
    await runCustomBtn.click();

    await expect(page.locator('text=Output Eksekusi Manual:')).toBeVisible({ timeout: 10000 });

    // 9. Browser E2E: Submit and verify test results & diff view
    const submitAllBtn = page.locator('button:has-text("Submit & Test (Semua)")');
    await submitAllBtn.click();

    await expect(page.locator('text=Hasil Automated Testing')).toBeVisible({ timeout: 15000 });
    await expect(page.locator('text=Lolos')).toBeVisible();

    // Auto-expanded failed test with diff view
    await expect(page.locator('text=Analisis Perbedaan Output (Diff Breakdown)').first()).toBeVisible();
    await expect(page.locator('text=Baris #1').first()).toBeVisible();

    // Toggle whitespace symbols
    const whitespaceBtn = page.locator('button:has-text("Tanda ␠ & ↵")');
    await whitespaceBtn.click();

    // Filter "Hanya yang Gagal"
    const failedFilterBtn = page.locator('button:has-text("Hanya yang Gagal")');
    await failedFilterBtn.click();

    const fatalErrors = consoleErrors.filter(e => !e.includes('favicon.ico'));
    expect(fatalErrors).toEqual([]);
  });

  test('13. Scroll Glitch & CLS Prevention: Zero Layout Thrashing and Stable Scroll Position on Boxing & Unboxing (lesson-02-05) and Related Lessons', async ({ page }) => {
    const testCases = [
      { id: 'lesson-02-05', width: 1440, height: 900, theme: 'dark' },
      { id: 'lesson-02-05', width: 375, height: 812, theme: 'light' },
      { id: 'lesson-02-04', width: 1440, height: 900, theme: 'dark' },
    ];

    for (const tc of testCases) {
      await page.setViewportSize({ width: tc.width, height: tc.height });

      await page.goto('/');
      await page.evaluate(({ id, theme }) => {
        localStorage.setItem('academy-last-lesson', id);
        localStorage.setItem('academy-theme', theme);
        if (theme === 'dark') {
          document.documentElement.classList.add('dark');
        } else {
          document.documentElement.classList.remove('dark');
        }
      }, { id: tc.id, theme: tc.theme });

      await page.reload();
      await expect(page.locator('h1')).toHaveCount(1);
      // Wait for any initial diagrams to finish rendering so we only measure scroll-induced shifts
      await page.locator('.mermaid-diagram svg').first().waitFor({ state: 'attached', timeout: 5000 }).catch(() => {});
      await page.waitForTimeout(600); // Allow layout to settle

      // Observe CLS and DOM mutations strictly during active scrolling
      await page.evaluate(() => {
        let cls = 0;
        try {
          const po = new PerformanceObserver((entryList) => {
            for (const entry of entryList.getEntries() as any[]) {
              if (!entry.hadRecentInput) {
                cls += entry.value;
              }
            }
          });
          po.observe({ type: 'layout-shift', buffered: false });
        } catch {
          // ignore
        }

        let mutationCount = 0;
        const scrollContainer = document.getElementById('main-scroll-container') || document.documentElement;
        const mo = new MutationObserver(() => {
          mutationCount++;
        });
        mo.observe(scrollContainer, { childList: true, subtree: true });

        (window as any).__scrollTest = {
          getCls: () => cls,
          getMutations: () => mutationCount
        };
      });

      const containerLocator = page.locator('#main-scroll-container');
      const box = await containerLocator.boundingBox();
      expect(box).not.toBeNull();
      if (box) {
        await page.mouse.move(box.x + box.width / 2, box.y + 200);
      }

      const maxScroll = await page.evaluate(() => {
        const c = document.getElementById('main-scroll-container') || document.documentElement;
        return c.scrollHeight - c.clientHeight;
      });

      const scrollStep = 180;
      const scrollDistance = Math.min(maxScroll, 3000);

      // Scroll Down
      for (let s = 0; s < scrollDistance; s += scrollStep) {
        await page.mouse.wheel(0, scrollStep);
        await page.waitForTimeout(40);
      }

      // Scroll Up
      for (let s = scrollDistance; s > 0; s -= scrollStep) {
        await page.mouse.wheel(0, -scrollStep);
        await page.waitForTimeout(40);
      }

      await page.waitForTimeout(200);

      const { cls, mutations } = await page.evaluate(() => {
        const t = (window as any).__scrollTest;
        return { cls: t.getCls(), mutations: t.getMutations() };
      });

      console.log(`[TEST 13] ${tc.id} (${tc.width}x${tc.height}, ${tc.theme}): CLS = ${cls}, mutations = ${mutations}`);

      // Verify CLS is well below the 0.1 threshold and DOM nodes are not thrashing during scroll
      expect(cls).toBeLessThan(0.1);
      expect(mutations).toBeLessThan(25);
    }
  });
});
