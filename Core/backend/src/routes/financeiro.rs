use axum::{
    extract::{Path, State},
    routing::get,
    Json, Router,
};
use chrono::Utc;
use futures::stream::TryStreamExt;
use mongodb::bson::{doc, oid::ObjectId};
use serde::Serialize;

use crate::{
    error::{AppError, Result},
    models::*,
    mongodb::MongoDb,
};

#[derive(Debug, Serialize)]
struct ContaResponse {
    pub id: Option<String>,
    pub nome: String,
    pub banco: String,
    pub tipo_conta: String,
    pub agencia: String,
    pub numero_conta: String,
    pub saldo_inicial: f64,
    pub saldo_atual: f64,
    pub ativo: bool,
}

impl From<ContaBancaria> for ContaResponse {
    fn from(conta: ContaBancaria) -> Self {
        Self {
            id: conta.id.map(|id| id.to_hex()),
            nome: format!("Conta {} {}", conta.banco, conta.conta),
            banco: conta.banco,
            tipo_conta: conta.tipo,
            agencia: conta.agencia,
            numero_conta: conta.conta,
            saldo_inicial: conta.saldo,
            saldo_atual: conta.saldo,
            ativo: true,
        }
    }
}

#[derive(Debug, Serialize)]
struct CartaoResponse {
    pub id: Option<String>,
    pub nome: String,
    pub bandeira: String,
    pub ultimos_digitos: String,
    pub limite_total: f64,
    pub limite_disponivel: f64,
    pub dia_vencimento: i32,
    pub dia_fechamento: i32,
    pub ativo: bool,
}

impl From<Cartao> for CartaoResponse {
    fn from(cartao: Cartao) -> Self {
        let ultimos_digitos = cartao
            .numero
            .chars()
            .rev()
            .take(4)
            .collect::<String>()
            .chars()
            .rev()
            .collect();

        let dia_fechamento = (cartao.vencimento - 7).max(1);

        Self {
            id: cartao.id.map(|id| id.to_hex()),
            nome: format!("Cartão {}", cartao.banco),
            bandeira: cartao.bandeira,
            ultimos_digitos,
            limite_total: cartao.limite,
            limite_disponivel: cartao.limite,
            dia_vencimento: cartao.vencimento,
            dia_fechamento,
            ativo: true,
        }
    }
}

pub fn routes(mongo: MongoDb) -> Router {
    Router::new()
        .route("/contas", get(list_contas).post(create_conta))
        .route(
            "/contas/:id",
            get(get_conta).put(update_conta).delete(delete_conta),
        )
        .route("/cartoes", get(list_cartoes).post(create_cartao))
        .route(
            "/cartoes/:id",
            get(get_cartao).put(update_cartao).delete(delete_cartao),
        )
        .with_state(mongo)
}

// === CONTAS BANCÁRIAS ===

async fn list_contas(State(mongo): State<MongoDb>) -> Result<Json<Vec<ContaResponse>>> {
    let contas: Vec<ContaBancaria> = mongo
        .contas_bancarias()
        .find(None, None)
        .await?
        .try_collect()
        .await?;
    Ok(Json(contas.into_iter().map(ContaResponse::from).collect()))
}

async fn get_conta(
    State(mongo): State<MongoDb>,
    Path(id): Path<String>,
) -> Result<Json<ContaResponse>> {
    let oid = ObjectId::parse_str(&id)?;
    let conta = mongo
        .contas_bancarias()
        .find_one(doc! { "_id": oid }, None)
        .await?
        .ok_or(AppError::NotFound)?;
    Ok(Json(conta.into()))
}

async fn create_conta(
    State(mongo): State<MongoDb>,
    Json(input): Json<CreateContaBancaria>,
) -> Result<Json<ContaResponse>> {
    let now = Utc::now();
    let conta = ContaBancaria {
        id: None,
        banco: input.banco,
        agencia: input.agencia,
        conta: input.conta,
        tipo: input.tipo,
        saldo: input.saldo,
        created_at: now,
        updated_at: now,
    };

    let result = mongo.contas_bancarias().insert_one(&conta, None).await?;
    let inserted_id = result.inserted_id.as_object_id().unwrap();

    let created = mongo
        .contas_bancarias()
        .find_one(doc! { "_id": inserted_id }, None)
        .await?
        .unwrap();

    Ok(Json(created.into()))
}

async fn update_conta(
    State(mongo): State<MongoDb>,
    Path(id): Path<String>,
    Json(input): Json<UpdateContaBancaria>,
) -> Result<Json<ContaResponse>> {
    let oid = ObjectId::parse_str(&id)?;

    let mut update_doc = doc! {
        "$set": {
            "updated_at": bson::DateTime::from_chrono(Utc::now())
        }
    };

    if let Some(banco) = input.banco {
        update_doc
            .get_document_mut("$set")
            .unwrap()
            .insert("banco", banco);
    }
    if let Some(agencia) = input.agencia {
        update_doc
            .get_document_mut("$set")
            .unwrap()
            .insert("agencia", agencia);
    }
    if let Some(conta) = input.conta {
        update_doc
            .get_document_mut("$set")
            .unwrap()
            .insert("conta", conta);
    }
    if let Some(tipo) = input.tipo {
        update_doc
            .get_document_mut("$set")
            .unwrap()
            .insert("tipo", tipo);
    }
    if let Some(saldo) = input.saldo {
        update_doc
            .get_document_mut("$set")
            .unwrap()
            .insert("saldo", saldo);
    }

    mongo
        .contas_bancarias()
        .update_one(doc! { "_id": oid }, update_doc, None)
        .await?;

    let updated = mongo
        .contas_bancarias()
        .find_one(doc! { "_id": oid }, None)
        .await?
        .ok_or(AppError::NotFound)?;

    Ok(Json(updated.into()))
}

async fn delete_conta(
    State(mongo): State<MongoDb>,
    Path(id): Path<String>,
) -> Result<Json<String>> {
    let oid = ObjectId::parse_str(&id)?;
    mongo
        .contas_bancarias()
        .delete_one(doc! { "_id": oid }, None)
        .await?;
    Ok(Json("Conta excluída".to_string()))
}

// === CARTÕES ===

async fn list_cartoes(State(mongo): State<MongoDb>) -> Result<Json<Vec<CartaoResponse>>> {
    let cartoes: Vec<Cartao> = mongo
        .cartoes()
        .find(None, None)
        .await?
        .try_collect()
        .await?;
    Ok(Json(cartoes.into_iter().map(CartaoResponse::from).collect()))
}

async fn get_cartao(
    State(mongo): State<MongoDb>,
    Path(id): Path<String>,
) -> Result<Json<CartaoResponse>> {
    let oid = ObjectId::parse_str(&id)?;
    let cartao = mongo
        .cartoes()
        .find_one(doc! { "_id": oid }, None)
        .await?
        .ok_or(AppError::NotFound)?;
    Ok(Json(cartao.into()))
}

async fn create_cartao(
    State(mongo): State<MongoDb>,
    Json(input): Json<CreateCartao>,
) -> Result<Json<CartaoResponse>> {
    let now = Utc::now();
    let cartao = Cartao {
        id: None,
        banco: input.banco,
        numero: input.numero,
        bandeira: input.bandeira,
        limite: input.limite,
        vencimento: input.vencimento,
        created_at: now,
        updated_at: now,
    };

    let result = mongo.cartoes().insert_one(&cartao, None).await?;
    let inserted_id = result.inserted_id.as_object_id().unwrap();

    let created = mongo
        .cartoes()
        .find_one(doc! { "_id": inserted_id }, None)
        .await?
        .unwrap();

    Ok(Json(created.into()))
}

async fn update_cartao(
    State(mongo): State<MongoDb>,
    Path(id): Path<String>,
    Json(input): Json<UpdateCartao>,
) -> Result<Json<CartaoResponse>> {
    let oid = ObjectId::parse_str(&id)?;

    let mut update_doc = doc! {
        "$set": {
            "updated_at": bson::DateTime::from_chrono(Utc::now())
        }
    };

    if let Some(banco) = input.banco {
        update_doc
            .get_document_mut("$set")
            .unwrap()
            .insert("banco", banco);
    }
    if let Some(numero) = input.numero {
        update_doc
            .get_document_mut("$set")
            .unwrap()
            .insert("numero", numero);
    }
    if let Some(bandeira) = input.bandeira {
        update_doc
            .get_document_mut("$set")
            .unwrap()
            .insert("bandeira", bandeira);
    }
    if let Some(limite) = input.limite {
        update_doc
            .get_document_mut("$set")
            .unwrap()
            .insert("limite", limite);
    }
    if let Some(vencimento) = input.vencimento {
        update_doc
            .get_document_mut("$set")
            .unwrap()
            .insert("vencimento", vencimento);
    }

    mongo
        .cartoes()
        .update_one(doc! { "_id": oid }, update_doc, None)
        .await?;

    let updated = mongo
        .cartoes()
        .find_one(doc! { "_id": oid }, None)
        .await?
        .ok_or(AppError::NotFound)?;

    Ok(Json(updated.into()))
}

async fn delete_cartao(
    State(mongo): State<MongoDb>,
    Path(id): Path<String>,
) -> Result<Json<String>> {
    let oid = ObjectId::parse_str(&id)?;
    mongo
        .cartoes()
        .delete_one(doc! { "_id": oid }, None)
        .await?;
    Ok(Json("Cartão excluído".to_string()))
}
