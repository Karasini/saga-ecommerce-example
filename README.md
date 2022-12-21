# SAGA shop checkout flow

Repository prepared to make a demo of SAGA pattern using shop checkout flow. Solution is prepared using [MassTransit library](https://masstransit-project.com/) where [SAGA framework](https://masstransit-project.com/usage/sagas/automatonymous.html) is built-in. 

## Business

![SAGA Flow](/docs/saga-shop-checkout-flow.jpeg)

Flow simulates simple shop checkout flow (happy path):
1. The user creates an order
2. The user pays for the order
3. After successfull payment, product is reserved
4. When product is reserved, delivery is booked
5. Finally the order is closed when it is delivered

Additional business rules that are required:
- The order is cancelled:
    - when payment fails three times
    - when delivery book fails
    - when product reservation fails
    - if it is not paid within the specified time
- Payment is refunded when order is cancelled
- If refund fails, then support need to handle it manually
- Products are reserved in the warehouse after successful payment
- If product does not exist in warehouse, order is cancelled and refund is made
- Delivery needs to be booked and it is handled by external company

## How to run
The solution uses MassTransit library which support multiple transport. By default project supports in memory transport and does not require any infrastructure dependencies.

But for debug purposes it is recommended to run SEQ log aggregator to see all logs from SAGA process.

### SEQ logs
To run standalone SEQ please run
```
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
```
Or run SEQ from docker-compose file in `SagaCheckoutFlow/Api/docker-compose.yaml` directory.
```
docker-compose up -d seq
```

