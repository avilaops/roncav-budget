# 🚀 Guia Rápido - Como Usar o Budget

## 📌 Iniciando o Sistema

### 1. Rodar o Servidor Django

No terminal, navegue até a pasta `django_app`:

```bash
cd django_app
python manage.py runserver 8080
```

Ou use o arquivo `.bat` (Windows):
```bash
RODAR.bat
```

### 2. Acessar o Sistema

Abra o navegador e acesse:
```
http://127.0.0.1:8080/
```

### 3. Fazer Login

Use as credenciais padrão:
- **Usuário**: `admin`
- **Senha**: `admin`

---

## 📚 Tutorial de Uso

### Passo 1: Criar Categorias 🏷️

1. Clique em **"Categorias"** no menu
2. Clique em **"+ Nova Categoria"**
3. Preencha:
   - Nome: Ex: "Alimentação", "Salário", "Transporte"
   - Tipo: Receita ou Despesa
   - Ícone: Escolha um emoji (🍔, 💼, 🚗, etc.)
   - Cor: Selecione uma cor para identificar
4. Clique em **"➕ Criar Categoria"**

**Sugestões de Categorias:**

**Receitas:**
- 💼 Salário
- 💰 Freelance
- 🎁 Bônus
- 📈 Investimentos

**Despesas:**
- 🍔 Alimentação
- 🏠 Moradia
- 🚗 Transporte
- 💊 Saúde
- 🎓 Educação
- 🎮 Lazer
- 👕 Vestuário
- 📱 Telecomunicações

---

### Passo 2: Adicionar Contas 🏦

1. Clique em **"Contas"** no menu
2. Clique em **"+ Nova Conta"**
3. Preencha:
   - Nome: Ex: "Nubank", "Banco do Brasil", "Carteira"
   - Tipo: Corrente, Poupança, Carteira ou Investimento
   - Saldo Inicial: Quanto você tem agora
   - Banco: (opcional)
   - Cor: Para identificar visualmente
4. Clique em **"➕ Criar Conta"**

---

### Passo 3: Registrar Transações 📝

1. Clique em **"Transações"** no menu
2. Clique em **"+ Nova Transação"**
3. Preencha:
   - Tipo: Receita ou Despesa
   - Descrição: Ex: "Compra no supermercado"
   - Valor: Quanto foi gasto/recebido
   - Data: Quando aconteceu
   - Conta: De onde saiu/entrou o dinheiro
   - Categoria: Classifique a transação
   - Observações: (opcional)
   - ☑️ Recorrente: Marque se é uma transação que se repete
4. Clique em **"➕ Criar Transação"**

**💡 Dica:** O saldo da conta é atualizado automaticamente!

---

### Passo 4: Definir Orçamentos 📊

1. Clique em **"Orçamentos"** no menu
2. Clique em **"+ Novo Orçamento"**
3. Preencha:
   - Categoria: Escolha uma categoria de DESPESA
   - Limite: Quanto você quer gastar no máximo
   - Mês/Ano: Para qual período
4. Clique em **"➕ Criar Orçamento"**

**Como funciona:**
- 🟢 Verde: Você está dentro do orçamento (0-79%)
- 🟡 Amarelo: Atenção! (80-99%)
- 🔴 Vermelho: Orçamento estourado! (100%+)

**Exemplo:**
- Categoria: 🍔 Alimentação
- Limite: R$ 1.000,00
- Mês: Dezembro/2025

---

### Passo 5: Criar Metas 🎯

1. Clique em **"Metas"** no menu
2. Clique em **"+ Nova Meta"**
3. Preencha:
   - Nome: Ex: "Viagem para Europa", "Carro novo"
   - Valor Alvo: Quanto você precisa juntar
   - Valor Atual: Quanto você já tem
   - Data de Início: Quando começou
   - Data Alvo: Quando quer atingir
4. Clique em **"➕ Criar Meta"**

**Acompanhamento:**
- Veja o progresso com barra visual
- Percentual completo calculado automaticamente
- Valor faltante para atingir a meta

**Exemplo:**
- Nome: 🏖️ Férias em Cancún
- Valor Alvo: R$ 10.000,00
- Valor Atual: R$ 2.500,00
- Data Alvo: 31/12/2025

---

### Passo 6: Acompanhar no Dashboard 📊

O Dashboard mostra:

1. **Cards de Resumo**
   - 💰 Saldo Total de todas as contas
   - ⬆️ Receitas do Mês
   - ⬇️ Despesas do Mês
   - 📈 Saldo do Mês (Receitas - Despesas)

2. **Suas Contas**
   - Lista de todas as contas ativas
   - Saldo atualizado de cada uma

3. **Orçamentos do Mês**
   - Progresso visual de cada orçamento
   - Quanto já foi gasto vs limite

4. **Metas Ativas**
   - Progresso de cada meta
   - Percentual completo

5. **Transações Recentes**
   - Últimas 10 transações

---

## ✏️ Editando e Deletando

### Para Editar:
1. Vá para a página do item (Transações, Contas, etc.)
2. Clique no botão **✏️** ao lado do item
3. Modifique os campos
4. Clique em **"💾 Salvar Alterações"**

### Para Deletar:
1. Clique no botão **🗑️** ao lado do item
2. Confirme a exclusão
3. O item será removido

**⚠️ Atenção:**
- Deletar uma transação reverte o saldo da conta
- Desativar uma conta não exclui as transações

---

## 🎨 Personalizando

### Cores
Escolha cores diferentes para cada conta e categoria para facilitar a identificação visual!

### Ícones
Use emojis para tornar suas categorias mais divertidas e fáceis de reconhecer:
- 🍔🍕🍝 Comida
- 🏠🏡🏢 Moradia
- 🚗🚕🚌 Transporte
- 💊⚕️🏥 Saúde
- 📚🎓✏️ Educação

---

## ❓ Dúvidas Comuns

**Q: Como criar um usuário novo?**
A: Use o Django Admin em `http://127.0.0.1:8080/admin/`

**Q: Posso ter várias contas?**
A: Sim! Adicione quantas precisar.

**Q: E se eu errar uma transação?**
A: Basta editar ou deletar, o saldo é ajustado automaticamente.

**Q: Como ver transações antigas?**
A: Todas ficam na página "Transações", ordenadas da mais recente.

**Q: Posso definir orçamento para receitas?**
A: Não, orçamentos são apenas para controlar gastos (despesas).

---

## 🆘 Suporte

Se encontrar problemas:
1. Verifique se o servidor está rodando
2. Confira as mensagens de erro na tela
3. Veja o console do terminal para logs

---

**Bom controle financeiro! 💰📊**
