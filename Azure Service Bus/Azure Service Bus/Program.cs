// package required to work with azure bus service -> azure.messaging.servicebus

using Azure.Messaging.ServiceBus;
using Azure_Service_Bus;
using Newtonsoft.Json;

//to connect to servicebus queue- take it from key valut
string connectionString = "";
string queueName = "appqueue";

List<Order> orders = new List<Order>()
{
    new Order(){OrderID="101",Quantity=100, UnitPrice=10},
    new Order(){OrderID="201",Quantity=200, UnitPrice=20},
    new Order(){OrderID="301",Quantity=300, UnitPrice=30}

};

//await SendMessage(orders);
//await PeekMessages();
//await ReceiveMessages();

async Task SendMessage(List<Order> orders)
{
    //ServiceBusClient allow us to connect to service bus
    ServiceBusClient serviceBusClient = new ServiceBusClient(connectionString);
    //ServiceBusSender allow us to send messages on our queue
    ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(queueName);
    //ServiceBusMessageBatch is uesd to send message in batch
    ServiceBusMessageBatch serviceBusMessageBatch = await serviceBusSender.CreateMessageBatchAsync();

    foreach (Order order in orders)
    {
        ServiceBusMessage serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(order));
        serviceBusMessage.ContentType= "application/json";
        if (!serviceBusMessageBatch.TryAddMessage(
            serviceBusMessage))

        {
            throw new Exception("Error Occured");
        }
    }

    Console.WriteLine("Message Sent");
    await serviceBusSender.SendMessagesAsync(serviceBusMessageBatch);
}

async Task PeekMessages()
{
    ServiceBusClient serviceBusClient = new ServiceBusClient(connectionString);
    //ServiceBusReceiver allow us to receieve message and load can be what is mode of receiving
    ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(queueName,new ServiceBusReceiverOptions() { ReceiveMode=ServiceBusReceiveMode.PeekLock});
    IAsyncEnumerable<ServiceBusReceivedMessage> messages=serviceBusReceiver.ReceiveMessagesAsync();

    await foreach(ServiceBusReceivedMessage message in messages)
    {
        Order order=JsonConvert.DeserializeObject<Order>(message.Body.ToString());

        Console.WriteLine("OrderId {0}", order.OrderID);
        Console.WriteLine("Quantity {0}", order.Quantity);
        Console.WriteLine("Unit Price {0}", order.UnitPrice);
    }

}



async Task ReceiveMessages()
{
    ServiceBusClient serviceBusClient = new ServiceBusClient(connectionString);
    //ServiceBusReceiver allow us to receieve message and load can be what is mode of receiving
    ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(queueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });
    IAsyncEnumerable<ServiceBusReceivedMessage> messages = serviceBusReceiver.ReceiveMessagesAsync();

    await foreach (ServiceBusReceivedMessage message in messages)
    {
        Order order = JsonConvert.DeserializeObject<Order>(message.Body.ToString());

        Console.WriteLine("OrderId {0}", order.OrderID);
        Console.WriteLine("Quantity {0}", order.Quantity);
        Console.WriteLine("Unit Price {0}", order.UnitPrice);
    }

}