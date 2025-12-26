db = db.getSiblingDB("erp");

// Limpar collection se existir
db.bancos.drop();

// Criar collection de bancos
db.createCollection("bancos");

// Criar índices
db.bancos.createIndex({ "codigo": 1 }, { unique: true });
db.bancos.createIndex({ "nome": "text" });
db.bancos.createIndex({ "pais": 1 });

// Inserir bancos do Brasil
db.bancos.insertMany([
    // Principais bancos comerciais
    { codigo: "001", nome: "Banco do Brasil S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "033", nome: "Banco Santander Brasil S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "104", nome: "Caixa Econômica Federal", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "237", nome: "Banco Bradesco S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "341", nome: "Itaú Unibanco S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "356", nome: "Banco Real S.A. (Santander)", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "745", nome: "Banco Citibank S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "399", nome: "HSBC Bank Brasil S.A.", pais: "BR", tipo: "comercial", ativo: true },

    // Bancos digitais
    { codigo: "260", nome: "Nu Pagamentos S.A. (Nubank)", pais: "BR", tipo: "digital", ativo: true },
    { codigo: "290", nome: "Pagseguro Internet S.A.", pais: "BR", tipo: "digital", ativo: true },
    { codigo: "323", nome: "Mercado Pago", pais: "BR", tipo: "digital", ativo: true },
    { codigo: "380", nome: "PicPay Serviços S.A.", pais: "BR", tipo: "digital", ativo: true },
    { codigo: "403", nome: "Cora SCD S.A.", pais: "BR", tipo: "digital", ativo: true },
    { codigo: "336", nome: "Banco C6 S.A.", pais: "BR", tipo: "digital", ativo: true },
    { codigo: "077", nome: "Banco Inter S.A.", pais: "BR", tipo: "digital", ativo: true },
    { codigo: "653", nome: "Banco Indusval S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "655", nome: "Banco Votorantim S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "422", nome: "Banco Safra S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "389", nome: "Banco Mercantil do Brasil S.A.", pais: "BR", tipo: "comercial", ativo: true },

    // Bancos de investimento
    { codigo: "208", nome: "Banco BTG Pactual S.A.", pais: "BR", tipo: "investimento", ativo: true },
    { codigo: "623", nome: "Banco Pan S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "212", nome: "Banco Original S.A.", pais: "BR", tipo: "digital", ativo: true },
    { codigo: "218", nome: "Banco BS2 S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "746", nome: "Banco Modal S.A.", pais: "BR", tipo: "investimento", ativo: true },

    // Cooperativas de crédito
    { codigo: "756", nome: "Banco Cooperativo do Brasil (BANCOOB)", pais: "BR", tipo: "cooperativa", ativo: true },
    { codigo: "748", nome: "Banco Cooperativo Sicredi S.A.", pais: "BR", tipo: "cooperativa", ativo: true },
    { codigo: "085", nome: "Cooperativa Central de Crédito - Ailos", pais: "BR", tipo: "cooperativa", ativo: true },
    { codigo: "136", nome: "Unicred Cooperativa", pais: "BR", tipo: "cooperativa", ativo: true },

    // Outros bancos brasileiros
    { codigo: "041", nome: "Banco do Estado do Rio Grande do Sul S.A.", pais: "BR", tipo: "estadual", ativo: true },
    { codigo: "047", nome: "Banco do Estado de Sergipe S.A.", pais: "BR", tipo: "estadual", ativo: true },
    { codigo: "070", nome: "Banco de Brasília S.A. (BRB)", pais: "BR", tipo: "estadual", ativo: true },
    { codigo: "021", nome: "Banco do Estado do Espírito Santo S.A.", pais: "BR", tipo: "estadual", ativo: true },
    { codigo: "004", nome: "Banco do Nordeste do Brasil S.A.", pais: "BR", tipo: "desenvolvimento", ativo: true },
    { codigo: "003", nome: "Banco da Amazônia S.A.", pais: "BR", tipo: "desenvolvimento", ativo: true },
    { codigo: "097", nome: "Cooperativa Central de Crédito Noroeste Brasileiro", pais: "BR", tipo: "cooperativa", ativo: true },
    { codigo: "741", nome: "Banco Ribeirão Preto S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "739", nome: "Banco Cetelem S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "743", nome: "Banco Semear S.A.", pais: "BR", tipo: "comercial", ativo: true },
    { codigo: "096", nome: "Banco B3 S.A.", pais: "BR", tipo: "infraestrutura", ativo: true },
]);

// Inserir bancos de Portugal
db.bancos.insertMany([
    { codigo: "0007", nome: "Banco Comercial Português (Millennium BCP)", pais: "PT", tipo: "comercial", ativo: true },
    { codigo: "0033", nome: "Banco Santander Totta S.A.", pais: "PT", tipo: "comercial", ativo: true },
    { codigo: "0035", nome: "Caixa Geral de Depósitos", pais: "PT", tipo: "comercial", ativo: true },
    { codigo: "0010", nome: "Banco BPI S.A.", pais: "PT", tipo: "comercial", ativo: true },
    { codigo: "0036", nome: "Montepio Geral", pais: "PT", tipo: "comercial", ativo: true },
    { codigo: "0079", nome: "Banco CTT S.A.", pais: "PT", tipo: "digital", ativo: true },
    { codigo: "0269", nome: "Banco Invest S.A.", pais: "PT", tipo: "investimento", ativo: true },
    { codigo: "0018", nome: "Banco BiG S.A.", pais: "PT", tipo: "digital", ativo: true },
    { codigo: "0099", nome: "ActivoBank", pais: "PT", tipo: "digital", ativo: true },
    { codigo: "0278", nome: "Moey! (Crédito Agrícola)", pais: "PT", tipo: "digital", ativo: true },
    { codigo: "0050", nome: "Crédito Agrícola", pais: "PT", tipo: "cooperativa", ativo: true },
    { codigo: "0023", nome: "Deutsche Bank (Portugal)", pais: "PT", tipo: "comercial", ativo: true },
]);

// Inserir bancos internacionais
db.bancos.insertMany([
    // Estados Unidos
    { codigo: "BOFA", nome: "Bank of America", pais: "US", tipo: "comercial", ativo: true },
    { codigo: "CITI", nome: "Citibank", pais: "US", tipo: "comercial", ativo: true },
    { codigo: "JPMC", nome: "JPMorgan Chase", pais: "US", tipo: "comercial", ativo: true },
    { codigo: "WELL", nome: "Wells Fargo", pais: "US", tipo: "comercial", ativo: true },
    { codigo: "GOLD", nome: "Goldman Sachs", pais: "US", tipo: "investimento", ativo: true },
    { codigo: "MORG", nome: "Morgan Stanley", pais: "US", tipo: "investimento", ativo: true },

    // Reino Unido
    { codigo: "HSBC", nome: "HSBC Holdings", pais: "GB", tipo: "comercial", ativo: true },
    { codigo: "BARC", nome: "Barclays", pais: "GB", tipo: "comercial", ativo: true },
    { codigo: "LLOY", nome: "Lloyds Banking Group", pais: "GB", tipo: "comercial", ativo: true },
    { codigo: "RBSG", nome: "NatWest Group (Royal Bank of Scotland)", pais: "GB", tipo: "comercial", ativo: true },
    { codigo: "STAN", nome: "Standard Chartered", pais: "GB", tipo: "comercial", ativo: true },

    // França
    { codigo: "BNPP", nome: "BNP Paribas", pais: "FR", tipo: "comercial", ativo: true },
    { codigo: "CRDA", nome: "Crédit Agricole", pais: "FR", tipo: "comercial", ativo: true },
    { codigo: "SOCG", nome: "Société Générale", pais: "FR", tipo: "comercial", ativo: true },

    // Alemanha
    { codigo: "DEUT", nome: "Deutsche Bank", pais: "DE", tipo: "comercial", ativo: true },
    { codigo: "COMZ", nome: "Commerzbank", pais: "DE", tipo: "comercial", ativo: true },

    // Espanha
    { codigo: "SANT", nome: "Banco Santander", pais: "ES", tipo: "comercial", ativo: true },
    { codigo: "BBVA", nome: "BBVA", pais: "ES", tipo: "comercial", ativo: true },
    { codigo: "CAIX", nome: "CaixaBank", pais: "ES", tipo: "comercial", ativo: true },

    // Suíça
    { codigo: "UBSW", nome: "UBS", pais: "CH", tipo: "investimento", ativo: true },
    { codigo: "CSGN", nome: "Credit Suisse", pais: "CH", tipo: "investimento", ativo: true },

    // Itália
    { codigo: "UCGM", nome: "UniCredit", pais: "IT", tipo: "comercial", ativo: true },
    { codigo: "ISPB", nome: "Intesa Sanpaolo", pais: "IT", tipo: "comercial", ativo: true },

    // China
    { codigo: "ICBC", nome: "Industrial and Commercial Bank of China", pais: "CN", tipo: "comercial", ativo: true },
    { codigo: "CCBC", nome: "China Construction Bank", pais: "CN", tipo: "comercial", ativo: true },
    { codigo: "ABOC", nome: "Agricultural Bank of China", pais: "CN", tipo: "comercial", ativo: true },
    { codigo: "BKCH", nome: "Bank of China", pais: "CN", tipo: "comercial", ativo: true },

    // Japão
    { codigo: "MUFG", nome: "Mitsubishi UFJ Financial Group", pais: "JP", tipo: "comercial", ativo: true },
    { codigo: "SMFG", nome: "Sumitomo Mitsui Financial Group", pais: "JP", tipo: "comercial", ativo: true },
    { codigo: "MIZH", nome: "Mizuho Financial Group", pais: "JP", tipo: "comercial", ativo: true },

    // Canadá
    { codigo: "RYAX", nome: "Royal Bank of Canada", pais: "CA", tipo: "comercial", ativo: true },
    { codigo: "TORC", nome: "Toronto-Dominion Bank", pais: "CA", tipo: "comercial", ativo: true },
    { codigo: "BKNS", nome: "Bank of Nova Scotia", pais: "CA", tipo: "comercial", ativo: true },

    // Austrália
    { codigo: "CBAAU", nome: "Commonwealth Bank of Australia", pais: "AU", tipo: "comercial", ativo: true },
    { codigo: "WBCAU", nome: "Westpac Banking Corporation", pais: "AU", tipo: "comercial", ativo: true },
    { codigo: "ANZAU", nome: "Australia and New Zealand Banking Group", pais: "AU", tipo: "comercial", ativo: true },

    // Outros
    { codigo: "INGB", nome: "ING Group", pais: "NL", tipo: "comercial", ativo: true },
    { codigo: "NWBK", nome: "Nordea Bank", pais: "SE", tipo: "comercial", ativo: true },
]);

// Verificar total inserido
print("\n✅ Bancos inseridos com sucesso!");
print("Total de bancos: " + db.bancos.countDocuments());
print("\nBancos por país:");
print("Brasil (BR): " + db.bancos.countDocuments({ pais: "BR" }));
print("Portugal (PT): " + db.bancos.countDocuments({ pais: "PT" }));
print("Internacional: " + db.bancos.countDocuments({ pais: { $nin: ["BR", "PT"] } }));

// Mostrar alguns exemplos
print("\n📋 Exemplos de bancos cadastrados:");
db.bancos.find().limit(5).forEach(printjson);
