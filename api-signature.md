# Assinatura da API aberta.

#### Neste projeto está contido um arquivo json que permite importar uma collection já configurada para importação no Postman.
- Arquivo para importar collection - https://github.com/pedr-munhoz/payment-api/blob/master/postman_requests_collection
- Link para baixar o Postman - https://www.postman.com/downloads/

### Recursos e métodos

#### Pagamentos
  - Url base
    - api/v2/payments
  - Efetuar pagamento com cartão de crédito;
	  - POST /
	  - body { string CreditCard, double RawValue, int PaymentInstallmentsCount }

  - Consultar uma transação e suas parcelas a partir do identificador da transação
	  - GET /{id:int}


#### Antecipações
  - url base
    - api/v2/anticipations
  - Consultar transações disponíveis para solicitar antecipação;
	  - Get /avaliable-payments

- Solicitar antecipação a partir de uma lista de transações;
	- POST /
	- body [id:int]

- Iniciar o atendimento da antecipação;
	- PATCH /start-analysis/{id:int}

- Aprovar uma ou mais transações da antecipação (quando todas as transações forem finalizadas, a antecipação será finalizada);
	- PATCH /{id:int}/approve
	  - body [int]

- Reprovar uma ou mais transações da antecipação (quando todas as transações forem finalizadas, a antecipação será finalizada);
	- PATCH /{id:int}/reject
	  - body [int]

- Consultar histórico de antecipações com filtro por status (pendente, em análise, finalizada).
	- Get /
	- query ?status (null, "pending", "analyzing", "finished")
