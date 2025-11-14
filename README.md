# 📈 Sistema de Monitoramento de Ativos

Sistema de monitoramento de preços de ativos da bolsa de valores brasileira (B3) que envia alertas por e-mail quando o preço atinge níveis de compra ou venda configurados.

## 🚀 Funcionalidades

- 🔍 Monitoramento em tempo real de ativos da B3
- 📊 Integração com a API Brapi para cotações
- 📧 Envio automático de alertas por e-mail
- ⚙️ Configuração personalizável de limites de compra e venda
- 🔄 Verificação periódica a cada 10 segundos

## 🛠️ Tecnologias

- **.NET 10.0** - Framework principal
- **C#** - Linguagem de programação
- **Brapi API** - API de cotações da bolsa brasileira
- **SMTP** - Protocolo para envio de e-mails
- **Microsoft.Extensions.Configuration** - Gerenciamento de configurações

## 📋 Pré-requisitos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Chave de API da [Brapi](https://brapi.dev/)
- Conta de e-mail com SMTP habilitado (ex: Gmail com senha de app)

## ⚙️ Configuração

1. **Clone o repositório**
   ```powershell
   git clone https://github.com/alisson94/asset-monitor-system-di.git
   cd asset-monitor-system-di
   ```

2. **Configure o arquivo `appsettings.json`**

   Renomeie `appsettings.template.json` para `appsettings.json` e edite com suas informações:
   ```json
   {
     "DestEmail": "seu-email-destino@email.com",
     "ApiKeyBrapi": "SUA_CHAVE_API_BRAPI",
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": 587,
       "Username": "seu-email@gmail.com",
       "Password": "SUA_SENHA_DE_APP"
     }
   }
   ```

   ### 📧 Configuração do Gmail (SMTP)
   
   Para usar o Gmail, você precisa gerar uma **senha de app**:
   1. Acesse sua [conta Google](https://myaccount.google.com/)
   2. Vá em **Segurança** → **Verificação em duas etapas** (ative se necessário)
   3. Procure por **Senhas de app**
   4. Crie uma senha para "Correio" ou "Outro"
   5. Use essa senha gerada no campo `Password` do `appsettings.json`

3. **Restaure as dependências**
   ```powershell
   dotnet restore
   ```

## 🎯 Como Usar

Execute o programa com os seguintes parâmetros:

```powershell
dotnet run <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>
```

### Parâmetros

- `<ATIVO>` - Código do ativo na B3 (ex: PETR4, VALE3, ITUB4)
- `<PRECO_VENDA>` - Preço limite para alerta de venda
- `<PRECO_COMPRA>` - Preço limite para alerta de compra

### Exemplos

Monitorar ações da Petrobras (PETR4):
```powershell
dotnet run PETR4 40,00 35,00
```

Monitorar ações do Itaú (ITUB4):
```powershell
dotnet run ITUB4 28,50 25,00
```

## 📊 Comportamento

O sistema verifica o preço do ativo a cada 10 segundos e:

- **Alerta de COMPRA** : Quando o preço atual está **abaixo** do preço de compra configurado
- **Alerta de VENDA** : Quando o preço atual está **acima** do preço de venda configurado
- **Monitoramento normal** : Quando o preço está dentro do intervalo configurado

Cada alerta envia um e-mail automático com:
- Nome do ativo
- Preço atual
- Percentual de variação em relação ao limite
- Recomendação (compra ou venda)

## 📁 Estrutura do Projeto

```
asset-monitor-system-di/
│
├── Program.cs              # Ponto de entrada e lógica principal
├── AssetPriceService.cs    # Serviço de consulta de preços (API Brapi)
├── EmailService.cs         # Serviço de envio de e-mails
├── BrapiResponseModel.cs   # Modelo de resposta da API Brapi
├── QuoteResultModel.cs     # Modelo de dados de cotação
├── SmtpSettings.cs         # Modelo de configurações SMTP
├── appsettings.json        # Arquivo de configuração
└── asset-monitor-system-di.csproj
```

## 📝 Exemplo de Saída

```
Monitorando PETR4 (Venda: R$ 40,00 | Compra: R$ 35,00)
Emails serão enviados para: usuario@email.com

==============================================

Alerta de COMPRA!
Preco atual (R$ 34,50) menor que o preco de compra (R$ 35,00)

Enviando e-mail para usuario@email.com...
E-mail enviado com sucesso!
```

---

