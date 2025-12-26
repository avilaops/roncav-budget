use axum::{routing::get, Json, Router};

use crate::models::{
    Cliente, DashboardData, EstoqueCritico, Produto, ProdutoMaisVendido, ResumoMes, Venda,
    VendasHoje,
};

pub fn routes() -> Router {
    Router::new()
        .route("/dashboard", get(get_dashboard))
        .route("/clientes", get(list_clientes))
        .route("/produtos", get(list_produtos))
        .route("/vendas", get(list_vendas))
}

async fn get_dashboard() -> Json<DashboardData> {
    Json(DashboardData {
        vendas_hoje: VendasHoje {
            quantidade: 12,
            valor_total: 4873.42,
        },
        estoque_critico: vec![
            EstoqueCritico {
                id: 1,
                nome: "Sensor IoT Omega".into(),
                estoque_atual: 4,
                estoque_minimo: 10,
            },
            EstoqueCritico {
                id: 2,
                nome: "Kit Segurança Industrial".into(),
                estoque_atual: 2,
                estoque_minimo: 8,
            },
        ],
        produtos_mais_vendidos: vec![
            ProdutoMaisVendido {
                produto_id: 1,
                produto_nome: "Placa Controladora Quantum".into(),
                total_vendido: 32,
                valor_total: 128_750.00,
            },
            ProdutoMaisVendido {
                produto_id: 2,
                produto_nome: "Módulo Edge AI Orion".into(),
                total_vendido: 21,
                valor_total: 94_320.00,
            },
        ],
        resumo_mes: ResumoMes {
            total_vendas: 87,
            valor_total: 243_875.32,
            ticket_medio: 2803.16,
        },
    })
}

async fn list_clientes() -> Json<Vec<Cliente>> {
    Json(vec![
        Cliente {
            id: 1,
            nome: "Carlos Lima".into(),
            cpf_cnpj: "098.765.432/0001-10".into(),
            telefone: Some("5511976543210".into()),
            email: Some("carlos@techone.com.br".into()),
            endereco: Some("Av. Paulista, 1000".into()),
            cidade: Some("São Paulo".into()),
            estado: Some("SP".into()),
            cep: Some("01310-100".into()),
            ativo: true,
        },
        Cliente {
            id: 2,
            nome: "Mariana Souza".into(),
            cpf_cnpj: "123.456.789-00".into(),
            telefone: Some("5511981234567".into()),
            email: Some("mariana@inova.dev".into()),
            endereco: Some("Rua Harmonia, 256".into()),
            cidade: Some("São Paulo".into()),
            estado: Some("SP".into()),
            cep: Some("05435-000".into()),
            ativo: true,
        },
    ])
}

async fn list_produtos() -> Json<Vec<Produto>> {
    Json(vec![
        Produto {
            id: 1,
            nome: "Placa Controladora Quantum".into(),
            descricao: Some("Controladora industrial com suporte a protocolos industriais Fieldbus".into()),
            codigo_barras: Some("1234567890123".into()),
            preco_custo: 1875.90,
            preco_venda: 2799.90,
            estoque_atual: 42,
            estoque_minimo: 15,
            unidade: "un".into(),
            ativo: true,
        },
        Produto {
            id: 2,
            nome: "Módulo Edge AI Orion".into(),
            descricao: Some("Processamento de visão computacional com acelerador neural integrado".into()),
            codigo_barras: Some("7894561230987".into()),
            preco_custo: 2890.50,
            preco_venda: 4490.00,
            estoque_atual: 18,
            estoque_minimo: 10,
            unidade: "un".into(),
            ativo: true,
        },
    ])
}

async fn list_vendas() -> Json<Vec<Venda>> {
    Json(vec![
        Venda {
            id: 1,
            cliente_id: Some(1),
            total: 5598.80,
            desconto: 0.0,
            total_final: 5598.80,
            forma_pagamento: "Cartão de Crédito".into(),
            status: "concluida".into(),
            observacoes: Some("Entrega programada para 48h".into()),
            usuario: Some("nicolas".into()),
        },
        Venda {
            id: 2,
            cliente_id: Some(2),
            total: 9490.00,
            desconto: 500.0,
            total_final: 8990.00,
            forma_pagamento: "PIX".into(),
            status: "faturada".into(),
            observacoes: None,
            usuario: Some("mariana".into()),
        },
    ])
}
