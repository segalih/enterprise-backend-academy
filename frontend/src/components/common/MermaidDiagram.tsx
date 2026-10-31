import React, { useEffect, useState, useRef } from 'react';
import { Copy, Check, AlertCircle } from 'lucide-react';

interface MermaidDiagramProps {
  code: string;
}

// In-memory cache to prevent layout shift and re-rendering on remounts
const mermaidSvgCache = new Map<string, string>();

export const MermaidDiagram: React.FC<MermaidDiagramProps> = ({ code }) => {
  const trimmed = code.trim();
  const cachedSvg = mermaidSvgCache.get(trimmed);
  const [svg, setSvg] = useState<string>(() => cachedSvg || '');
  const [error, setError] = useState<string | null>(null);
  const [copied, setCopied] = useState<boolean>(false);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    let isMounted = true;

    const render = async () => {
      try {
        setError(null);
        const isDark = document.documentElement.classList.contains('dark');
        const mermaidModule = await import('mermaid');
        const mermaid = mermaidModule.default;
        (window as any).__mermaid = mermaid;

        mermaid.initialize({
          startOnLoad: false,
          theme: isDark ? 'dark' : 'neutral',
          securityLevel: 'strict',
          fontFamily: 'ui-sans-serif, system-ui, sans-serif',
          themeVariables: isDark
            ? {
                darkMode: true,
                background: '#090d16',
                mainBkg: '#0f172a',
                nodeBorder: '#334155',
                textColor: '#f1f5f9',
                lineColor: '#38bdf8',
                secondaryColor: '#1e293b',
                tertiaryColor: '#0b1120'
              }
            : {
                darkMode: false,
                background: '#ffffff',
                mainBkg: '#f8fafc',
                nodeBorder: '#cbd5e1',
                textColor: '#0f172a',
                lineColor: '#0284c7',
                secondaryColor: '#e2e8f0',
                tertiaryColor: '#f1f5f9'
              }
        });

        const renderId = `mermaid_${Date.now()}_${Math.random().toString(36).substring(2, 8)}`;
        const { svg: renderedSvg } = await mermaid.render(renderId, trimmed);
        mermaidSvgCache.set(trimmed, renderedSvg);
        if (isMounted) setSvg(renderedSvg);
      } catch (err: any) {
        console.warn('Mermaid rendering error:', err);
        if (isMounted) setError(err?.message || 'Gagal merender diagram Mermaid');
      }
    };

    // If not cached, or on theme change, render
    if (!cachedSvg) {
      render();
    }

    const observer = new MutationObserver((mutations) => {
      for (const m of mutations) {
        if (m.type === 'attributes' && m.attributeName === 'class') {
          render();
          break;
        }
      }
    });

    observer.observe(document.documentElement, { attributes: true });
    return () => {
      isMounted = false;
      observer.disconnect();
    };
  }, [code, trimmed, cachedSvg]);

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(code);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch {
      // ignore fallback
    }
  };

  if (error) {
    return (
      <div className="my-5 rounded-xl border border-amber-800/60 bg-amber-950/20 p-4">
        <div className="flex items-center space-x-2 text-xs font-semibold text-amber-400 mb-2">
          <AlertCircle className="w-4 h-4" />
          <span>Diagram Mermaid (Syntax Fallback)</span>
        </div>
        <pre className="p-3 text-xs font-mono bg-slate-900 rounded overflow-x-auto text-slate-300">
          <code>{code}</code>
        </pre>
      </div>
    );
  }

  return (
    <div className="my-6 rounded-xl border border-slate-800/90 bg-[#090d16] p-4 sm:p-6 shadow-xl shadow-black/20 overflow-hidden" data-testid="mermaid-diagram">
      <div className="flex items-center justify-between pb-3 border-b border-slate-800/80 mb-4 select-none">
        <div className="flex items-center space-x-2">
          <span className="w-2 h-2 rounded-full bg-blue-500 animate-pulse" />
          <span className="text-[11px] font-mono uppercase tracking-wider font-semibold text-blue-400">
            Diagram Arsitektur Interaktif
          </span>
        </div>
        <button
          type="button"
          onClick={handleCopy}
          title="Salin definisi Mermaid"
          className="flex items-center space-x-1.5 px-2 py-0.5 rounded bg-slate-800/80 hover:bg-slate-700/80 text-slate-300 hover:text-white border border-slate-700/60 transition text-[11px]"
        >
          {copied ? (
            <>
              <Check className="w-3 h-3 text-emerald-400" />
              <span className="text-emerald-300">Tersalin</span>
            </>
          ) : (
            <>
              <Copy className="w-3 h-3 text-slate-400" />
              <span>Salin</span>
            </>
          )}
        </button>
      </div>

      <div
        ref={containerRef}
        className="w-full min-h-[120px] flex justify-center items-center overflow-x-auto py-2 [&_svg]:max-w-full [&_svg]:h-auto text-slate-100"
        aria-label="Diagram alur sistem"
        dangerouslySetInnerHTML={{ __html: svg }}
      />
    </div>
  );
};
