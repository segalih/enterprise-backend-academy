import React, { useState } from 'react';
import { Binary, RefreshCw, Cpu } from 'lucide-react';

export const BinarySimulator: React.FC = () => {
  const [bits, setBits] = useState<number[]>([0, 0, 1, 0, 1, 0, 1, 0]); // default 42

  const toggleBit = (index: number) => {
    const next = [...bits];
    next[index] = next[index] === 0 ? 1 : 0;
    setBits(next);
  };

  const setAll = (val: number) => {
    setBits(new Array(8).fill(val));
  };

  // Calculations
  const unsignedDecimal = bits.reduce((acc, bit, idx) => acc + bit * Math.pow(2, 7 - idx), 0);
  
  // Two's complement for 8-bit signed
  const isNegative = bits[0] === 1;
  const signedDecimal = isNegative 
    ? - (Math.pow(2, 7) - bits.slice(1).reduce((acc, bit, idx) => acc + bit * Math.pow(2, 6 - idx), 0))
    : unsignedDecimal;

  const hexValue = unsignedDecimal.toString(16).toUpperCase().padStart(2, '0');

  const setNumber = (num: number) => {
    const clamped = Math.max(0, Math.min(255, num));
    const newBits = [];
    for (let i = 7; i >= 0; i--) {
      newBits.push((clamped >> i) & 1);
    }
    setBits(newBits);
  };

  return (
    <div className="bg-slate-900 text-slate-100 rounded-xl p-6 border border-slate-800 shadow-xl my-6">
      <div className="flex items-center justify-between pb-4 border-b border-slate-800 mb-6">
        <div className="flex items-center space-x-3">
          <div className="p-2 bg-blue-600/20 text-blue-400 rounded-lg">
            <Binary className="w-5 h-5" />
          </div>
          <div>
            <h3 className="font-semibold text-lg text-white">8-Bit Binary & Two's Complement Simulator</h3>
            <p className="text-xs text-slate-400">Klik setiap bit untuk mengubah statusnya (0/1) dan lihat pengaruhnya secara instan.</p>
          </div>
        </div>
        <div className="flex space-x-2">
          <button 
            onClick={() => setNumber(42)} 
            className="text-xs px-2.5 py-1 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded border border-slate-700 transition"
          >
            Nilai 42
          </button>
          <button 
            onClick={() => setNumber(255)} 
            className="text-xs px-2.5 py-1 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded border border-slate-700 transition"
          >
            Maks (255)
          </button>
          <button 
            onClick={() => setAll(0)} 
            className="text-xs px-2.5 py-1 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded border border-slate-700 transition flex items-center space-x-1"
          >
            <RefreshCw className="w-3 h-3" />
            <span>Reset</span>
          </button>
        </div>
      </div>

      {/* Bit Switches */}
      <div className="mb-6">
        <div className="text-xs font-mono text-slate-400 mb-2 flex justify-between px-1">
          <span>Bit 7 (MSB - Sign Bit)</span>
          <span>Bit 0 (LSB - Nilai 1)</span>
        </div>
        <div className="grid grid-cols-8 gap-2 sm:gap-3">
          {bits.map((bit, idx) => {
            const power = 7 - idx;
            const weight = Math.pow(2, power);
            const isSignBit = idx === 0;
            return (
              <div key={idx} className="flex flex-col items-center">
                <button
                  onClick={() => toggleBit(idx)}
                  className={`w-full aspect-square rounded-lg font-mono text-xl sm:text-2xl font-bold flex items-center justify-center transition-all duration-150 transform active:scale-95 border ${
                    bit === 1
                      ? isSignBit 
                        ? 'bg-amber-600 border-amber-400 text-white shadow-lg shadow-amber-900/40'
                        : 'bg-blue-600 border-blue-400 text-white shadow-lg shadow-blue-900/40'
                      : 'bg-slate-800/80 border-slate-700 text-slate-400 hover:bg-slate-800'
                  }`}
                >
                  {bit}
                </button>
                <span className="text-[10px] font-mono text-slate-400 mt-1">2^{power}</span>
                <span className="text-[10px] font-mono text-slate-500">({isSignBit ? '-128/+128' : weight})</span>
              </div>
            );
          })}
        </div>
      </div>

      {/* Numerical Results Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 bg-slate-950/60 p-4 rounded-xl border border-slate-800/80">
        <div className="p-3 bg-slate-900/80 rounded-lg border border-slate-800">
          <div className="text-xs text-slate-400">Unsigned Decimal (byte)</div>
          <div className="text-2xl font-mono font-bold text-blue-400 mt-1">{unsignedDecimal}</div>
          <div className="text-[11px] text-slate-500 mt-1">Rentang: 0 s.d 255</div>
        </div>

        <div className="p-3 bg-slate-900/80 rounded-lg border border-slate-800">
          <div className="text-xs text-slate-400">Signed (sbyte - Two's Complement)</div>
          <div className={`text-2xl font-mono font-bold mt-1 ${signedDecimal < 0 ? 'text-amber-400' : 'text-emerald-400'}`}>
            {signedDecimal}
          </div>
          <div className="text-[11px] text-slate-500 mt-1">Rentang: -128 s.d +127</div>
        </div>

        <div className="p-3 bg-slate-900/80 rounded-lg border border-slate-800">
          <div className="text-xs text-slate-400">Hexadecimal Representation</div>
          <div className="text-2xl font-mono font-bold text-purple-400 mt-1">0x{hexValue}</div>
          <div className="text-[11px] text-slate-500 mt-1">1 Byte = 2 Digit Hex</div>
        </div>
      </div>

      {/* Explanation Box */}
      <div className="mt-4 p-3 bg-blue-950/30 border border-blue-900/40 rounded-lg text-xs text-blue-300 flex items-start space-x-2">
        <Cpu className="w-4 h-4 mt-0.5 shrink-0 text-blue-400" />
        <div>
          <span className="font-semibold text-blue-200">Catatan Internal Arsitektur Komputer:</span> Pada representasi Two's Complement, jika Bit 7 bernilai <span className="font-mono font-bold">1</span>, CPU menginterpretasikannya sebagai bilangan bertanda negatif. Aritmatika biner Two's Complement memungkinkan ALU melakukan pengurangan cukup dengan sirkuit penjumlah (adder) yang sama!
        </div>
      </div>
    </div>
  );
};
