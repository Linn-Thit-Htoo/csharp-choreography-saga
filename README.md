## Distributed Transactions (SAGA Pattern)

It's extremely straightforward to handle multiple transactions in traditional monolith. What if we have 10 microservices and what if the 10th one fails then we have to rollback the other transactions to maintain consistency. Distributed transactions sound more complex huh?


Saga breaks down the entire transaction into the small, manageable, isolated transactions, each handled by the different microservices and communicate via events. There are two types of Saga: Choreography & Orchestrator. Each approach comes with its own trade-offs

### Choreography-Based Approach (Event-Driven)

There is no central coordinator in this approach. So, there's no SPOF :3. Each microservice knows what to do next, which action to trigger by publishing events. The scriber does not need to know about the producer and vice versa. All the microservices perform their own actions based on the events. If one service fails, it will emit the failure event and other microservices consumes it and perform compensating actions. Imagine Customer places an order, order service processes that order then the inventory service reverses the stock then finally payment service charges the customer. In this case, what if payment integration fails (may be timeout, unreachable whether you use retry mechanism or circuit breaker)? If payment service fails to process then it will emit the payment failure event then all other microservices (order microservice and inventory microservice in this case) will compensate based on that event.

### Orchestration-Based Approach

The centralized approach for handling transactions which simply means a single coordinator controls and manages the entire Saga. The orchestrator tells each service when to start, what to do, and when to proceed to the next step. The orchestrator handles failures and compensating actions if needed. The biggest con is it's single point of failure. The con is that debugging is more easier than the Choreography-Based Approach.
