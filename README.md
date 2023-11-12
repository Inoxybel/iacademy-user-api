# Documentação da API

## Visão Geral

Esta API é um módulo do conjunto de módulos da IAcademy que tem como objetivo suportar funcionalidades de gerenciamento de usuários e empresas.

## Endpoints
- Empresa
  - [Recuperar Empresa](#recuperar-empresa)
  - [Atualizar Empresa](#atualizar-empresa)
  - [Cadastrar Nova Empresa](#cadastrar-nova-empresa)
  - [Login da Empresa](#login-da-empresa)
  - [Atualizar Senha da Empresa](#atualizar-senha-da-empresa)
- Feedback
  - [Cadastrar Feedback](#cadastrar-feedback)
- Planos
  - [Recuperar Plano](#recuperar-plano)
  - [Atualizar Plano](#atualizar-plano)
  - [Criar Novo Plano](#criar-novo-plano)
- Usuário
  - [Recuperar](#recuperar-usuário)
  - [Atualizar](#atualizar-usuário)
  - [Cadastrar](#criar-novo-usuário)
  - [Login](#logar)
  - [Atualizar Senha](#atualizar-senha)
  - [Ativar Cadastro](#ativar-usuário)


## Company

#### Recuperar Empresa

**GET** `/api/company/{companyId}`

**Respostas:**
- **200 Sucesso** - Retorna a empresa correspondente ao companyId informado.
- **400 Requisição Inválida** - A solicitação foi inválida.
- **404 Não Encontrado** - Empresa não encontrada.

#### Atualizar Empresa

**PUT** `/api/company/{companyId}`

**Requisição:**

```
{
  "name": "Another Company Name",
  "cnpj": "12345678901234",
  "password": "P@ssw0rd",
  "confirmPassword": "P@ssw0rd",
  "groups": [
    {
      "groupName": "Sector One",
      "users": [
        {
          "name": "Mr. Smith",
          "document": "12345678901"
        }
      ],
      "authorizedTrainingIds": [
        "8629c8cf-d0a6-4a8a-8893-929c751b6a8a",
        "42fcaed1-fb7a-4e57-9e9d-87c48a2e25d2",
        "415796fd-0420-4835-aabd-68731f20e2b3",
        "c4ac6068-68f0-471a-97f6-33755e7cb7d0",
        "7befa9ff-1ed7-473a-bd3d-8a545c0ca3f6"
      ]
    }
  ]
}
```

**Respostas:**
- **204 Sem Conteúdo** - Atualização realizada com sucesso.
- **400 Requisição Inválida** - A solicitação foi inválida.
- **404 Não Encontrado** - Empresa não encontrada.

#### Cadastrar Nova Empresa

**POST** `/api/company`

**Requisição:**

```
{
  "companyDetails": "..."
}
```

**Respostas:**
- **201 Criado** - Retorna o ID da empresa criada.
- **400 Requisição Inválida** - A solicitação foi inválida.

#### Login da Empresa

**POST** `/api/company/login`

**Requisição:**

```
{
  "loginCredentials": "..."
}
```

**Respostas:**
- **200 OK** - Login bem-sucedido, retorna dados da empresa.
- **400 Requisição Inválida** - Credenciais inválidas.
- **404 Não Encontrado** - Empresa não encontrada.

#### Atualizar Senha da Empresa

**PUT** `/api/company/{companyId}/update-password`

**Requisição:**

```
{
  "passwordDetails": "..."
}
```

**Respostas:**
- **204 Sem Conteúdo** - Senha atualizada com sucesso.
- **400 Requisição Inválida** - Campos inválidos ou senha antiga incorreta.
- **401 Não Autorizado** - Empresa não autenticada.
- **404 Não Encontrado** - Empresa não encontrada.

## Feedback

#### Cadastrar Feedback

**POST** `/api/feedback`

**Requisição:**

```
{
  "feedbackDetails": "..."
}
```

**Respostas:**
- **201 Criado** - Retorna o ID do feedback criado.
- **400 Requisição Inválida** - A solicitação foi inválida.

## Plano

#### Recuperar Plano

**GET** `/api/plan/{planId}`

**Respostas:**
- **200 Sucesso** - Retorna detalhes do plano correspondente ao planId informado.
- **404 Não Encontrado** - Plano não encontrado.

#### Atualizar Plano

**PUT** `/api/plan/{planId}`

**Requisição:**

```
{
  "planUpdateDetails": "..."
}
```

**Respostas:**
- **200 Sucesso** - Atualização realizada com sucesso.
- **404 Não Encontrado** - Plano não encontrado.

#### Criar Novo Plano

**POST** `/api/plan`

**Requisição:**

```
{
  "planCreationDetails": "..."
}
```

**Respostas:**
- **200 Sucesso** - Retorna ID do novo plano criado.
- **404 Não Encontrado** - Erro na criação do plano.

## Usuário

### Recuperar Usuário

**GET** `/api/user/{userId}`

#### Respostas:

- **201 Criado** - Retorna o usuário correspondente ao userId informado.
- **400 Requisição Inválida** - A solicitação foi inválida.
- **401 Token Inválido** - Token inválido.
- **404 Não encontrado** - Usuário não encontrado.

### Atualizar Usuário

**PUT** `/api/user/{userId}`

#### Requisição:

```json
{
  "name": "Thiago Matos",
  "email": "thiago@IAcademy.tech",
  "companyRef": "iacademy",
  "password": "CurrentP@ssw0rd"
}
```

#### Respostas:

- **204 Atualizado** - Atualizado com sucesso.
- **400 Requisição Inválida** - A solicitação foi inválida.
- **401 Token Inválido** - Token inválido.
- **404 Não encontrado** - Usuário não encontrado.

### Criar Novo Usuário

**POST** `/api/user`

#### Requisição:

```json
{
  "name": "Thiago Matos",
  "email": "thiago@iacademy.tech",
  "cpf": "09746147005",
  "cellphoneNumberWithDDD": "11945343471",
  "password": "@Senha123",
  "confirmPassword": "@Senha123",
  "companyRef": ""
}
```

#### Respostas:

- **201 Criado** - Retorna o ID único do usuário criado.
- **400 Requisição Inválida** - A solicitação foi inválida.

### Logar

**POST** `/api/user/login`

#### Requisição:

```json
{
  "email": "thiago@iacademy.tech",
  "password": "@Senha123"
}
```

#### Respostas:

- **200 OK** - Autenticação foi bem-sucedida, retorna dados do usuário e token.
- **400 Requisição Inválida** - Credenciais inválidas ou campos faltantes.
- **404 Não Encontrado** - Usuário não encontrado.

### Atualizar Senha

**PUT** `/api/user/{id}/update-password`

#### Requisição:

```json
{
  "email": "thiago@iacademy.tech",
  "oldPassword": "@Senha123",
  "newPassword": "Novasenha1.",
  "confirmPassword": "Novasenha1."
}
```

#### Respostas:

- **204 Sem Conteúdo** - Senha atualizada com sucesso.
- **400 Requisição Inválida** - Campos inválidos ou senha antiga incorreta.
- **401 Não Autorizado** - Usuário não está autenticado.
- **404 Não Encontrado** - Usuário não encontrado.

### Ativar Usuário

**POST** `/api/user/{id}/active/{activationCode}`

#### Respostas:

- **200 OK** - Usuário ativado com sucesso.
- **400 Requisição Inválida** - Código de ativação inválido ou problemas na requisição.


## Evidências

- [Acesse esse link](https://youtu.be/WwOwKbZlqY8 "Acesse esse link") para ver a esteira de CI e CD em funcionamento