import { test, expect } from '@playwright/test';

test.describe('Scroll Glitch Investigation & Measurement', () => {

  const testCases = [
    { id: 'lesson-02-05', name: 'Boxing & Unboxing (Reported Glitch)', width: 1440, height: 900, theme: 'dark' },
    { id: 'lesson-02-05', name: 'Boxing & Unboxing Mobile Dark', width: 375, height: 812, theme: 'dark' },
    { id: 'lesson-02-05', name: 'Boxing & Unboxing Desktop Light', width: 1440, height: 900, theme: 'light' },
    { id: 'lesson-02-05', name: 'Boxing & Unboxing Mobile Light', width: 375, height: 812, theme: 'light' },
    { id: 'lesson-02-04', name: 'Value vs Reference Types', width: 1440, height: 900, theme: 'dark' },
    { id: 'lesson-00-04', name: 'Binary & Hexadecimal', width: 1440, height: 900, theme: 'dark' },
  ];

  for (const tc of testCases) {
    test(`Diagnose scroll behavior on ${tc.id} (${tc.name})`, async ({ page }) => {
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
      await page.waitForTimeout(1000); // Wait for initial render and diagrams

      // Attach diagnostic monitors in browser context
      const diagnostics = await page.evaluate(async () => {
        // Observe DOM mutations in main-scroll-container to count unmount/remount operations
        const scrollContainer = document.getElementById('main-scroll-container') || document.documentElement;
        let mutationCount = 0;
        let addedNodesCount = 0;
        let removedNodesCount = 0;

        const mo = new MutationObserver((mutations) => {
          for (const m of mutations) {
            mutationCount++;
            addedNodesCount += m.addedNodes.length;
            removedNodesCount += m.removedNodes.length;
          }
        });
        mo.observe(scrollContainer, { childList: true, subtree: true });

        // Monitor scrollTo, scrollBy, scrollIntoView
        const origScrollTo = Element.prototype.scrollTo;
        const origScrollBy = Element.prototype.scrollBy;
        const origScrollIntoView = Element.prototype.scrollIntoView;
        const origWindowScrollTo = window.scrollTo;

        const calls: any[] = [];
        Element.prototype.scrollTo = function(...args) {
          calls.push({ fn: 'element.scrollTo', args, stack: new Error().stack?.split('\n').slice(1, 4).join(' | ') });
          return origScrollTo.apply(this, args as any);
        };
        Element.prototype.scrollBy = function(...args) {
          calls.push({ fn: 'element.scrollBy', args, stack: new Error().stack?.split('\n').slice(1, 4).join(' | ') });
          return origScrollBy.apply(this, args as any);
        };
        Element.prototype.scrollIntoView = function(...args) {
          calls.push({ fn: 'element.scrollIntoView', target: this.id || this.tagName, stack: new Error().stack?.split('\n').slice(1, 4).join(' | ') });
          return origScrollIntoView.apply(this, args as any);
        };
        window.scrollTo = function(...args) {
          calls.push({ fn: 'window.scrollTo', args, stack: new Error().stack?.split('\n').slice(1, 4).join(' | ') });
          return origWindowScrollTo.apply(this, args as any);
        };

        // Layout shift observer
        let cumulativeLayoutShift = 0;
        const layoutShifts: any[] = [];
        try {
          const po = new PerformanceObserver((entryList) => {
            for (const entry of entryList.getEntries() as any[]) {
              if (!entry.hadRecentInput) {
                cumulativeLayoutShift += entry.value;
                const sources = (entry.sources || []).map((s: any) => ({
                  node: s.node ? `${s.node.nodeName || s.node.tagName || 'node'}#${s.node.id || ''}.${String(s.node.className || '').slice(0, 40)}` : 'unknown',
                  currentRect: s.currentRect,
                  previousRect: s.previousRect
                }));
                layoutShifts.push({ value: entry.value, sources });
              }
            }
          });
          po.observe({ type: 'layout-shift', buffered: true });
        } catch {
          // ignore
        }

        // Scroll jump recorder
        const scrollRecords: any[] = [];
        let prevScrollTop = scrollContainer.scrollTop;
        let prevScrollHeight = scrollContainer.scrollHeight;

        const recordScroll = (phase: string, requestedDelta: number) => {
          const currentTop = scrollContainer.scrollTop;
          const currentHeight = scrollContainer.scrollHeight;
          const actualDelta = currentTop - prevScrollTop;
          const unexpectedJump = Math.abs(actualDelta - requestedDelta);

          if (unexpectedJump > 5) {
            scrollRecords.push({
              phase,
              prevScrollTop,
              currentTop,
              requestedDelta,
              actualDelta,
              unexpectedJump,
              prevScrollHeight,
              currentHeight
            });
          }
          prevScrollTop = currentTop;
          prevScrollHeight = currentHeight;
        };

        (window as any).__scrollDiag = {
          scrollContainerInfo: {
            id: scrollContainer.id,
            tagName: scrollContainer.tagName,
            clientHeight: scrollContainer.clientHeight,
            scrollHeight: scrollContainer.scrollHeight,
            overflowY: window.getComputedStyle(scrollContainer).overflowY
          },
          recordScroll,
          getResults: () => ({
            cls: cumulativeLayoutShift,
            layoutShifts,
            calls,
            scrollRecords,
            mutationCount,
            addedNodesCount,
            removedNodesCount,
            finalScrollHeight: scrollContainer.scrollHeight
          })
        };

        return (window as any).__scrollDiag.scrollContainerInfo;
      });

      console.log(`[${tc.id} - ${tc.name}] Scroll container info:`, diagnostics);

      // Position mouse in the center of main-scroll-container so mouse.wheel targets it
      const containerLocator = page.locator('#main-scroll-container');
      const box = await containerLocator.boundingBox();
      if (box) {
        await page.mouse.move(box.x + box.width / 2, box.y + 200);
      }

      console.log(`[${tc.id} - ${tc.name}] Container box:`, box);

      // Now perform gradual scrolling downwards and upwards
      const scrollStep = 150;
      const maxScroll = await page.evaluate(() => {
        const c = document.getElementById('main-scroll-container') || document.documentElement;
        return c.scrollHeight - c.clientHeight;
      });

      console.log(`[${tc.id} - ${tc.name}] Max scroll distance: ${maxScroll}px`);

      // 1. Scroll Down gradually using wheel
      for (let s = 0; s < Math.min(maxScroll, 3000); s += scrollStep) {
        await page.mouse.wheel(0, scrollStep);
        await page.waitForTimeout(60);
        await page.evaluate((step) => {
          (window as any).__scrollDiag.recordScroll('wheel-down', step);
        }, scrollStep);
      }

      await page.waitForTimeout(300);

      // 2. Scroll Up gradually using wheel
      for (let s = Math.min(maxScroll, 3000); s > 0; s -= scrollStep) {
        await page.mouse.wheel(0, -scrollStep);
        await page.waitForTimeout(60);
        await page.evaluate((step) => {
          (window as any).__scrollDiag.recordScroll('wheel-up', -step);
        }, scrollStep);
      }

      await page.waitForTimeout(300);

      // Collect diagnostic results
      const results = await page.evaluate(() => {
        return (window as any).__scrollDiag.getResults();
      });

      console.log(`[${tc.id} - ${tc.name}] RESULTS:`, {
        cls: results.cls,
        layoutShiftsCount: results.layoutShifts.length,
        mutationCount: results.mutationCount,
        addedNodesCount: results.addedNodesCount,
        removedNodesCount: results.removedNodesCount,
        unexpectedJumpsCount: results.scrollRecords.length,
        layoutShiftsDetail: JSON.stringify(results.layoutShifts.map((ls: any) => ({
          val: ls.value,
          src: (ls.sources || []).map((s: any) => s.node)
        })), null, 2)
      });
    });
  }
});
