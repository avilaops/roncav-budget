use axum::{extract::State, routing::get, Json, Router};
use futures::stream::TryStreamExt;
use mongodb::bson::doc;

use crate::{error::Result, models::Banco, mongodb::MongoDb};

pub fn routes(mongo: MongoDb) -> Router {
    Router::new()
        .route("/", get(list_bancos))
        .route("/seed", get(seed_bancos))
        .with_state(mongo)
}

async fn list_bancos(State(mongo): State<MongoDb>) -> Result<Json<Vec<Banco>>> {
    let bancos: Vec<Banco> = mongo
        .bancos()
        .find(doc! { "ativo": true }, None)
        .await?
        .try_collect()
        .await?;
    Ok(Json(bancos))
}

async fn seed_bancos(State(mongo): State<MongoDb>) -> Result<Json<String>> {
    mongo.bancos().drop(None).await.ok();

    let bancos = vec![
        // Brasil
        Banco {
            id: None,
            codigo: "001".into(),
            nome: "Banco do Brasil S.A.".into(),
            pais: "BR".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "033".into(),
            nome: "Banco Santander Brasil S.A.".into(),
            pais: "BR".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "104".into(),
            nome: "Caixa Econômica Federal".into(),
            pais: "BR".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "237".into(),
            nome: "Banco Bradesco S.A.".into(),
            pais: "BR".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "341".into(),
            nome: "Itaú Unibanco S.A.".into(),
            pais: "BR".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "260".into(),
            nome: "Nu Pagamentos S.A. (Nubank)".into(),
            pais: "BR".into(),
            tipo: "digital".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "290".into(),
            nome: "Pagseguro Internet S.A.".into(),
            pais: "BR".into(),
            tipo: "digital".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "323".into(),
            nome: "Mercado Pago".into(),
            pais: "BR".into(),
            tipo: "digital".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "380".into(),
            nome: "PicPay Serviços S.A.".into(),
            pais: "BR".into(),
            tipo: "digital".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "336".into(),
            nome: "Banco C6 S.A.".into(),
            pais: "BR".into(),
            tipo: "digital".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "077".into(),
            nome: "Banco Inter S.A.".into(),
            pais: "BR".into(),
            tipo: "digital".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "212".into(),
            nome: "Banco Original S.A.".into(),
            pais: "BR".into(),
            tipo: "digital".into(),
            ativo: true,
        },
        // Portugal
        Banco {
            id: None,
            codigo: "0007".into(),
            nome: "Millennium BCP".into(),
            pais: "PT".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "0033".into(),
            nome: "Santander Totta".into(),
            pais: "PT".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "0035".into(),
            nome: "Caixa Geral de Depósitos".into(),
            pais: "PT".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "0010".into(),
            nome: "Banco BPI".into(),
            pais: "PT".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        // Internacional
        Banco {
            id: None,
            codigo: "HSBC".into(),
            nome: "HSBC Holdings".into(),
            pais: "GB".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "CITI".into(),
            nome: "Citibank".into(),
            pais: "US".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "JPMC".into(),
            nome: "JPMorgan Chase".into(),
            pais: "US".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
        Banco {
            id: None,
            codigo: "BOFA".into(),
            nome: "Bank of America".into(),
            pais: "US".into(),
            tipo: "comercial".into(),
            ativo: true,
        },
    ];

    mongo.bancos().insert_many(&bancos, None).await?;
    let total = mongo.bancos().count_documents(None, None).await?;

    Ok(Json(format!("✅ {} bancos inseridos!", total)))
}
