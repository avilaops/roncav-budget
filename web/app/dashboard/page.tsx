// ============================================================
// 📊 Dashboard Principal
// ============================================================

'use client';

import { useEffect, useState } from 'react';
import { useAuth } from '@/hooks/useAuth';
import { supabase, formatCurrency } from '@/lib/supabase';
import { useRouter } from 'next/navigation';

export default function DashboardPage() {
  const { user, signOut } = useAuth();
  const router = useRouter();
  const [saldoTotal, setSaldoTotal] = useState(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user) {
      router.push('/login');
      return;
    }

    async function loadData() {
      const { data: contas } = await supabase
        .from('contas')
        .select('saldo_atual')
        .eq('user_id', user.id);

      const total = contas?.reduce((acc, c) => acc + Number(c.saldo_atual), 0) || 0;
      setSaldoTotal(total);
      setLoading(false);
    }

    loadData();
  }, [user, router]);

  if (loading) {
    return <div className="flex items-center justify-center h-screen">Carregando...</div>;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="bg-white shadow p-4">
        <div className="max-w-7xl mx-auto flex justify-between items-center">
          <h1 className="text-2xl font-bold">💰 Orçamento Familiar</h1>
          <button onClick={signOut} className="text-gray-600 hover:text-gray-900">
            Sair →
          </button>
        </div>
      </header>

      <main className="max-w-7xl mx-auto p-8">
        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-sm text-gray-500 mb-2">Saldo Total</h2>
          <p className="text-4xl font-bold text-blue-600">
            {formatCurrency(saldoTotal)}
          </p>
        </div>
      </main>
    </div>
  );
}
