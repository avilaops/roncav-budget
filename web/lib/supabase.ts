// ============================================================
// 🔐 Cliente Supabase - Orçamento Familiar
// ============================================================

import { createClient } from '@supabase/supabase-js';

const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL!;
const supabaseAnonKey = process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY!;

export const supabase = createClient(supabaseUrl, supabaseAnonKey);

// Tipos
export type Conta = {
  id: string;
  user_id: string;
  nome: string;
  saldo_atual: number;
  created_at: string;
};

export type Transacao = {
  id: string;
  user_id: string;
  conta_id: string;
  descricao: string;
  valor: number;
  tipo: 'Receita' | 'Despesa';
  data_transacao: string;
};

// Funções auxiliares
export const formatCurrency = (value: number) =>
  new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);

export const formatDate = (date: string) =>
  new Date(date).toLocaleDateString('pt-BR');
