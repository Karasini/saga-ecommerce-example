# SAGA shop checkout flow

Repository prepared to make a demo of SAGA pattern using shop checkout flow. Solution is prepared using [MassTransit library](https://masstransit-project.com/) where [SAGA framework](https://masstransit-project.com/usage/sagas/automatonymous.html) is built-in. 

## Business

![SAGA Flow](/docs/saga-shop-checkout-flow.jpeg)

Flow simulates shop checkout flow in simple way, where some business rules need to be fullfiled:
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