use chrono::{DateTime, Utc};
use mongodb::bson::oid::ObjectId;
use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize, Clone)]
pub struct ContaBancaria {
    #[serde(rename = "_id", skip_serializing_if = "Option::is_none")]
    pub id: Option<ObjectId>,
    pub banco: String,
    pub agencia: String,
    pub conta: String,
    pub tipo: String,
    pub saldo: f64,
    #[serde(with = "bson::serde_helpers::chrono_datetime_as_bson_datetime")]
    pub created_at: DateTime<Utc>,
    #[serde(with = "bson::serde_helpers::chrono_datetime_as_bson_datetime")]
    pub updated_at: DateTime<Utc>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct CreateContaBancaria {
    pub banco: String,
    pub agencia: String,
    pub conta: String,
    pub tipo: String,
    pub saldo: f64,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct UpdateContaBancaria {
    pub banco: Option<String>,
    pub agencia: Option<String>,
    pub conta: Option<String>,
    pub tipo: Option<String>,
    pub saldo: Option<f64>,
}

#[derive(Debug, Serialize, Deserialize, Clone)]
pub struct Cartao {
    #[serde(rename = "_id", skip_serializing_if = "Option::is_none")]
    pub id: Option<ObjectId>,
    pub banco: String,
    pub numero: String,
    pub bandeira: String,
    pub limite: f64,
    pub vencimento: i32,
    #[serde(with = "bson::serde_helpers::chrono_datetime_as_bson_datetime")]
    pub created_at: DateTime<Utc>,
    #[serde(with = "bson::serde_helpers::chrono_datetime_as_bson_datetime")]
    pub updated_at: DateTime<Utc>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct CreateCartao {
    pub banco: String,
    pub numero: String,
    pub bandeira: String,
    pub limite: f64,
    pub vencimento: i32,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct UpdateCartao {
    pub banco: Option<String>,
    pub numero: Option<String>,
    pub bandeira: Option<String>,
    pub limite: Option<f64>,
    pub vencimento: Option<i32>,
}
