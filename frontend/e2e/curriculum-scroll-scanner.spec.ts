import { test, expect } from '@playwright/test';

test.describe('Automated Curriculum Scroll & CLS Scanner Across Levels', () => {

  // Representative sample covering different levels, diagram types, simulators, and content lengths
  const sampleLessons = [
    { id: 'lesson-00-01', name: 'Computer Anatomy (CPU Simulator)' },
    { id: 'lesson-00-04', name: 'Binary & Hex (Binary Simulator + Challenge)' },
    { id: 'lesson-01-04', name: 'Process Anatomy & State Transition' },
    { id: 'lesson-02-04', name: 'Value vs Reference Types (Mermaid + Stack/Heap Simulator)' },
    { id: 'lesson-02-05', name: 'Boxing & Unboxing (Mermaid Sequence + Simulator)' },
    { id: 'lesson-03-03', name: 'Polymorphism & vtable' },
    { id: 'lesson-04-02', name: 'Dynamic Arrays & Capacity' },
    { id: 'lesson-05-04', name: 'DNS Architecture & Hierarchy' },
    { id: 'lesson-06-03', name: 'LOH & POH Memory Fragmentation' },
    { id: 'lesson-07-01', name: 'Span<T> and Memory<T>' },
    { id: 'lesson-08-02', name: 'B-Tree & B+Tree Deep Dive' },
    { id: 'lesson-09-02', name: 'LINQ to Entities Expression Trees' },
    { id: 'lesson-10-02', name: 'ASP.NET Core Middleware Pipeline (Mermaid)' },
    { id: 'lesson-11-01', name: 'Threading vs Async (Mermaid Sequence)' },
    { id: 'lesson-12-04', name: 'Bounded Channels (Concurrency Simulator)' },
    { id: 'lesson-13-01', name: 'Cache-Aside Pattern' },
    { id: 'lesson-14-02', name: 'Message Broker vs Event Streaming' },
    { id: 'lesson-15-02', name: 'PKCE Code Challenge & SHA256' },
    { id: 'lesson-16-02', name: 'Structured Logging Serilog' },
    { id: 'lesson-17-01', name: 'Docker Internals & Namespaces' },
    { id: 'lesson-18-02', name: 'CQRS & Event Sourcing Architecture' },
  ];

  for (const l of sampleLessons) {
    test(`Scan ${l.id} (${l.name}) for Scroll Glitch and CLS < 0.1`, async ({ page }) => {
      await page.setViewportSize({ width: 1440, height: 900 });

      await page.goto('/');
      await page.evaluate((lessonId) => {
        localStorage.setItem('academy-last-lesson', lessonId);
        localStorage.setItem('academy-theme', 'dark');
      }, l.id);

      await page.reload();
      await expect(page.locator('h1')).toHaveCount(1);
      await page.waitForTimeout(600); // Allow initial layout & diagrams to stabilize

      // Attach CLS observer
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
          po.observe({ type: 'layout-shift', buffered: true });
        } catch {
          // ignore
        }
        (window as any).__scannerCls = () => cls;
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

      // Scroll down and up
      const scrollStep = 200;
      const scrollDistance = Math.min(maxScroll, 3000);

      for (let s = 0; s < scrollDistance; s += scrollStep) {
        await page.mouse.wheel(0, scrollStep);
        await page.waitForTimeout(40);
      }

      for (let s = scrollDistance; s > 0; s -= scrollStep) {
        await page.mouse.wheel(0, -scrollStep);
        await page.waitForTimeout(40);
      }

      await page.waitForTimeout(200);

      const finalCls = await page.evaluate(() => {
        return (window as any).__scannerCls ? (window as any).__scannerCls() : 0;
      });

      console.log(`[Scanner] ${l.id} final CLS: ${finalCls}`);
      expect(finalCls).toBeLessThan(0.1);
    });
  }
});
