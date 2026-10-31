import React, { useState, useEffect } from 'react';
import { Bookmark, BookmarkCheck, CheckCircle2, Circle, ExternalLink, Link2 } from 'lucide-react';
import { CrossReference } from '../../types/academy';

interface RightTocProps {
  headings: Array<{ id: string; text: string; level: number }>;
  activeHeading?: string;
  onScrollTo?: (id: string) => void;
  isBookmarked: boolean;
  onToggleBookmark: () => void;
  isCompleted: boolean;
  onToggleComplete: () => void;
  relatedLessons: CrossReference[];
  onSelectLesson: (id: string) => void;
  officialReferences?: Array<string | { title: string; url: string; type?: string }>;
}

export const RightToc: React.FC<RightTocProps> = ({
  headings,
  activeHeading: externalActiveHeading,
  onScrollTo,
  isBookmarked,
  onToggleBookmark,
  isCompleted,
  onToggleComplete,
  relatedLessons,
  onSelectLesson,
  officialReferences
}) => {
  const [internalActiveHeading, setInternalActiveHeading] = useState<string>('');
  const activeHeading = externalActiveHeading || internalActiveHeading || headings[0]?.id || '';

  // Self-contained scroll-spy: tracks scroll without triggering App or LessonPage re-renders
  useEffect(() => {
    const scrollContainer = document.getElementById('main-scroll-container');
    if (!scrollContainer || headings.length === 0) return;

    let ticking = false;

    const handleScroll = () => {
      if (ticking) return;
      ticking = true;

      requestAnimationFrame(() => {
        ticking = false;
        const headingElements = headings
          .map(h => document.getElementById(h.id))
          .filter((el): el is HTMLElement => el !== null);

        if (headingElements.length === 0) return;

        const containerRect = scrollContainer.getBoundingClientRect();
        let currentActive = headingElements[0].id;

        for (const el of headingElements) {
          const rect = el.getBoundingClientRect();
          if (rect.top - containerRect.top <= 120) {
            currentActive = el.id;
          } else {
            break;
          }
        }

        setInternalActiveHeading(prev => (prev === currentActive ? prev : currentActive));
      });
    };

    scrollContainer.addEventListener('scroll', handleScroll, { passive: true });
    handleScroll();
    return () => scrollContainer.removeEventListener('scroll', handleScroll);
  }, [headings]);

  const handleHeadingClick = (id: string) => {
    if (onScrollTo) {
      onScrollTo(id);
    } else {
      const el = document.getElementById(id);
      if (el) {
        el.scrollIntoView({ behavior: 'smooth' });
      }
    }
    setInternalActiveHeading(id);
  };
  return (
    <aside data-testid="right-toc" className="w-72 shrink-0 sticky top-16 h-[calc(100vh-4rem)] overflow-y-auto p-4 border-l border-slate-800 bg-slate-950/60 hidden xl:block text-slate-300">
      {/* Actions: Complete & Bookmark */}
      <div className="space-y-2 pb-4 border-b border-slate-800 mb-6">
        <button
          onClick={onToggleComplete}
          className={`w-full py-2 px-3 rounded-lg text-xs font-semibold flex items-center justify-center space-x-2 transition ${
            isCompleted
              ? 'bg-emerald-600/20 border border-emerald-500/60 text-emerald-300'
              : 'bg-emerald-600 hover:bg-emerald-500 text-white shadow-lg shadow-emerald-900/30'
          }`}
        >
          {isCompleted ? (
            <>
              <CheckCircle2 className="w-4 h-4 text-emerald-400" />
              <span>Selesai Dipelajari</span>
            </>
          ) : (
            <>
              <Circle className="w-4 h-4" />
              <span>Tandai Selesai</span>
            </>
          )}
        </button>

        <button
          onClick={onToggleBookmark}
          className={`w-full py-1.5 px-3 rounded-lg text-xs font-medium border flex items-center justify-center space-x-2 transition ${
            isBookmarked
              ? 'bg-blue-600/20 border-blue-500 text-blue-300'
              : 'bg-slate-900 hover:bg-slate-800 border-slate-800 text-slate-300'
          }`}
        >
          {isBookmarked ? (
            <>
              <BookmarkCheck className="w-3.5 h-3.5 text-blue-400" />
              <span>Tersimpan di Bookmark</span>
            </>
          ) : (
            <>
              <Bookmark className="w-3.5 h-3.5" />
              <span>Simpan Bookmark</span>
            </>
          )}
        </button>
      </div>

      {/* Table of Contents */}
      <div className="mb-6">
        <div className="text-[11px] font-mono uppercase tracking-wider text-slate-400 font-bold mb-3">
          Daftar Isi Materi
        </div>
        {headings.length === 0 ? (
          <div className="text-xs text-slate-500 italic">Materi sedang dimuat...</div>
        ) : (
          <nav className="space-y-1">
            {headings.map((h, idx) => (
              <button
                key={`${h.id}-${idx}`}
                onClick={() => handleHeadingClick(h.id)}
                className={`w-full text-left text-xs transition truncate block py-1 ${
                  h.level === 3 ? 'pl-4' : 'pl-1'
                } ${
                  activeHeading === h.id
                    ? 'text-blue-400 font-semibold border-l-2 border-blue-500 pl-2'
                    : 'text-slate-400 hover:text-slate-200'
                }`}
              >
                {h.text}
              </button>
            ))}
          </nav>
        )}
      </div>

      {/* Cross-References */}
      {relatedLessons && relatedLessons.length > 0 && (
        <div className="mb-6 pt-4 border-t border-slate-800">
          <div className="text-[11px] font-mono uppercase tracking-wider text-slate-400 font-bold mb-3 flex items-center space-x-1.5">
            <Link2 className="w-3.5 h-3.5 text-purple-400" />
            <span>Cross-Reference Materi</span>
          </div>
          <div className="space-y-2">
            {relatedLessons.map((rel, i) => (
              <div
                key={i}
                onClick={() => onSelectLesson(rel.lessonId)}
                className="p-2 rounded-lg bg-slate-900/80 hover:bg-slate-800 border border-slate-800/80 cursor-pointer transition text-xs"
              >
                <div className="font-semibold text-slate-200 truncate">{rel.title}</div>
                <div className="text-[10px] text-purple-300 font-mono mt-0.5">{rel.levelName}</div>
                <div className="text-[10px] text-slate-400 mt-1 line-clamp-2">{rel.context}</div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Official References */}
      {officialReferences && officialReferences.length > 0 && (
        <div className="pt-4 border-t border-slate-800">
          <div className="text-[11px] font-mono uppercase tracking-wider text-slate-400 font-bold mb-3 flex items-center space-x-1.5">
            <ExternalLink className="w-3.5 h-3.5 text-emerald-400" />
            <span>Referensi Terkait</span>
          </div>
          <div className="space-y-2">
            {officialReferences.map((refItem, i) => {
              const isObj = typeof refItem === 'object' && refItem !== null;
              const href = isObj ? refItem.url : refItem;
              const title = isObj ? refItem.title : refItem.replace('https://', '');
              return (
                <a
                  key={i}
                  href={href}
                  target="_blank"
                  rel="noreferrer"
                  className="text-xs text-blue-400 hover:text-blue-300 hover:underline flex items-start space-x-1.5 group"
                >
                  <ExternalLink className="w-3 h-3 text-slate-500 group-hover:text-blue-400 shrink-0 mt-0.5" />
                  <span className="line-clamp-2">{title}</span>
                </a>
              );
            })}
          </div>
        </div>
      )}
    </aside>
  );
};
