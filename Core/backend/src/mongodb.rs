use mongodb::{Client, Collection, Database};

#[derive(Clone)]
pub struct MongoDb {
    db: Database,
}

impl MongoDb {
    pub async fn new(uri: &str, db_name: &str) -> Result<Self, mongodb::error::Error> {
        let client = Client::with_uri_str(uri).await?;
        let db = client.database(db_name);
        Ok(Self { db })
    }

    pub fn contas_bancarias(&self) -> Collection<crate::models::ContaBancaria> {
        self.db.collection("contas_bancarias")
    }

    pub fn cartoes(&self) -> Collection<crate::models::Cartao> {
        self.db.collection("cartoes")
    }

    pub fn bancos(&self) -> Collection<crate::models::Banco> {
        self.db.collection("bancos")
    }
}
