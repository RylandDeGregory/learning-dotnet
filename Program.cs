using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage.Models;
using Azure.ResourceManager.Storage;

var location = AzureLocation.EastUS2;
string resourceGroupName = "myResourceGroup";
string storageAccountName = $"samplestor{Random.Shared.Next(1000)}";

ArmClient client = new ArmClient(new DefaultAzureCredential());
SubscriptionResource subscription = client.GetDefaultSubscription();
Console.WriteLine($"Subscription ID: {subscription.Data.DisplayName}");

var resourceGroups = subscription.GetResourceGroups();
ResourceGroupResource? resourceGroup = null;

try
{
    var resourceGroupData = new ResourceGroupData(location);
    Console.WriteLine($"Creating Resource Group: {resourceGroupName}");
    var resourceGroupOperation = await resourceGroups.CreateOrUpdateAsync(WaitUntil.Completed, resourceGroupName, resourceGroupData);
    resourceGroup = resourceGroupOperation.Value;
    Console.WriteLine($"Resource Group Created: {resourceGroup.Data.Id}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error creating resource group: {ex.Message}");
    return;
}

try
{
    Console.WriteLine($"Create Storage Account: {resourceGroupName}");
    var storageOperation = await resourceGroup.GetStorageAccounts().CreateOrUpdateAsync(
        WaitUntil.Completed,
        storageAccountName,
        new StorageAccountCreateOrUpdateContent(
            kind: StorageKind.StorageV2,
            sku: new StorageSku(StorageSkuName.StandardLrs),
            location: location
        )
    );
    var storageAccount = storageOperation.Value;
    Console.WriteLine($"Storage Account Created: {storageAccount.Data.Id}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error creating storage account: {ex.Message}");
    return;
}