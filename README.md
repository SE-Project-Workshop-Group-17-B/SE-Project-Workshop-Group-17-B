![Trade System Project](documents/Version3/ProjectPicture.jpg)
# Trade System Project

Comprehensive framework for managing trading activities, from user registration to store management.

## Group Members
Gal Pinto - pigal@post.bgu.ac.il

## Features

The Trade System Project is a versatile system designed to streamline trading activities. It includes features such as:

- **User Registration**: Supports registration of admins, subscribers, and other user roles.
- **Store Management**: Facilitates opening and managing multiple stores with unique inventories.
- **Product Management**: Allows detailed management of products in stores.
- **Role Assignment**: Enables assignment of roles like managers and owners to users.
- **User Authentication**: Includes functionalities for logging in and out users.
- **Future Enhancements**: Plans for additional functionalities and improvements.

## Installation

### Pre-requisites

The following are required to set up the Trade System Project:

- Visual Studio 2022
- .NET Core 3.1 or higher

### Steps to Install

1. **Clone the Repository**

    Open Visual Studio 2022 and clone the repository using the following URL:
    ```bash
    https://github.com/SE-Project-Workshop-Group-17-B/SE-Project-Workshop-Group-17-B
    ```

2. **Open the Solution**

    Once the repository is cloned, open the solution file (`.sln`) in Visual Studio 2022.

3. **Restore Dependencies**

    Restore the necessary dependencies by right-clicking on the solution in Solution Explorer and selecting `Restore NuGet Packages`.

4. **Build the Solution**

    Build the solution by pressing `Ctrl+Shift+B` or by selecting `Build > Build Solution` from the menu.

### Running the System

To run both the frontend and the API simultaneously:

1. **Set Multiple Startup Projects**

    - Right-click on the solution in Solution Explorer and select `Properties`.
    - Under the `Startup Project` section, select `Multiple startup projects`.
    - Set both the frontend and API projects to `Start`.

2. **Run the Solution**

    Press `F5` or click the `Start` button to run both projects. This will launch both the frontend and the API simultaneously.

## Configuration Files

The system requires specific configuration files to initialize properly. These files define the necessary settings and initial states for the application.

### Configuration File

The configuration file (`config_generate.json`) defines various settings such as whether to generate data, load from a database, or use an in-memory database. It also includes user registration, store management, product management, and role assignment configurations.

Example format of `config_generate.json`:

```json
{
  "generateData": false,
  "loadFromDB": false,
  "isMemoryDB": true,
  "register admins": {
    "req name": "generate admins",
    "actions": [
      {
        "action name": "register admin",
        "registration type": "admin",
        "username": "admin1",
        "password": "secureAdminPassword1"
      }
    ]
  },
  "register subscribers": {
    "req name": "generate subscribers",
    "actions": [
      {
        "action name": "register subscriber",
        "registration type": "subscriber",
        "username": "user1",
        "password": "UserPassword123!"
      },
      ...
    ]
  },
  "open stores": [
    {
      "req name": "open stores",
      "username": "user1",
      "password": "UserPassword123!",
      "actions": [
        {
          "action name": "open store",
          "store name": "Gourmet Shop",
          "store email": "gourmetshop@example.com",
          "store phone": "123-456-7890",
          "store description": "A shop specializing in gourmet foods and ingredients.",
          "store address": "123 Culinary St, Food City"
        },
        ...
      ]
    },
    ...
  ],
  "add products": [
    {
      "req name": "add products",
      "user type": "subscriber",
      "username": "user1",
      "password": "UserPassword123!",
      "store name": "Gourmet Shop",
      "actions": [
        {
          "action name": "add product",
          "product name": "Truffle Oil",
          "product description": "Premium truffle oil for gourmet cooking.",
          "product category": "Food",
          "product amount": 10,
          "product price": 20
        },
        ...
      ]
    }
  ],
  "assign managers": {
    "req name": "assign managers",
    "user type": "subscriber",
    "username": "user1",
    "password": "UserPassword123!",
    "actions": [
      {
        "action name": "assign manager",
        "store name": "Gourmet Shop",
        "new manager": "user4",
        "authorizations": [
          "View",
          "UpdateSupply"
        ]
      }
    ]
  },
  "assign owners": {
    "req name": "assign owners",
    "user type": "founder",
    "username": "user1",
    "password": "UserPassword123!",
    "actions": [
      {
        "action name": "assign owner",
        "store name": "Gourmet Shop",
        "new owner": "user5"
      },
      ...
    ]
  },
  "logout users": {
    "req name": "logout users",
    "actions": [
      {
        "action name": "logout user",
        "username": "user2"
      },
      ...
    ]
  }
}
```

### Initial State File

The initial state file specifies the initial state of the system, including pre-registered users, stores, and products. The initialization process interacts with the service layer and ensures all actions are legal.

Example format for initializing users and stores:

```
guest-registration(moshe, *moshe’s password*, *other required information*);
guest-registration(rina, *rina’s password*, *other required information*);
login(rina, *password*);
open-shop(rina, shoes, *other required information*);
appoint-manager(rina, shoes, moshe, *other required information*);
```

The initialization from the initial state file is successful only if all actions are legal and complete successfully. If any action fails, the initialization process must fail and report the error accordingly.

### How to Use Configuration Files

1. Place the configuration files in the appropriate directory (e.g., `../Sadna-17-B/Layer_Infrastructure/config_generate.json`).
2. Ensure the configuration file path is correctly set in the `Config.cs` file.

### Configuration Class

The `Config` class in `Config.cs` handles the loading and execution of configuration settings.

Example from `Config.cs`:

```csharp
public class Config
{
    public static string config_file_path = @"../Sadna-17-B/Layer_Infrastructure/config_generate.json";
    
    // JSON properties
    [JsonPropertyName("generateData")]
    public bool generateData { get; set; }

    [JsonPropertyName("loadFromDB")]
    public bool loadFromDB { get; set; }

    [JsonPropertyName("isMemoryDB")]
    public bool is_memory { get; set; }

    [JsonPropertyName("register admins")]
    public Requirement_register register_admins { get; set; }

    [JsonPropertyName("register subscribers")]
    public Requirement_register register_subscribers { get; set; }

    [JsonPropertyName("open stores")]
    public List<Requirement_open_store> open_stores { get; set; }

    [JsonPropertyName("add products")]
    public List<Requirement_add_product> add_products { get; set; }

    [JsonPropertyName("assign managers")]
    public Requirement_assign_managers assign_managers { get; set; }

    [JsonPropertyName("assign owners")]
    public Requirement_assign_owners assign_owners { get; set; }

    [JsonPropertyName("logout users")]
    public Requirement_logout logout_users { get; set; }

    // Local variables
    public static UserService user_service;
    public static StoreService store_service;
    public static Dictionary<string, string> user_to_token = new Dictionary<string, string>();
    public static Dictionary<string, int> storeNam_to_id = new Dictionary<string, int>();

    public void set_services(UserService userService, StoreService storeService)
    {
        user_service = userService;
        store_service = storeService;
    }

    public void execute_requirements()
    {
        if (register_admins != null)
            foreach (Action_register action in register_admins.actions)
                action.apply_action(user_service, store_service, register_admins);

        if (register_subscribers != null)
            foreach (Action_register action in register_subscribers.actions)
                action.apply_action(user_service, store_service, register_subscribers);

        if (open_stores != null)
            foreach (Requirement_open_store requirement in open_stores)
                foreach (Action_open_store action in requirement.actions)
                    action.apply_action(user_service, store_service, requirement);

        if (add_products != null)
            foreach (Requirement_add_product requirement in add_products)
                foreach (Action_add_product action in requirement.actions)
                    action.apply_action(user_service, store_service, requirement);

        if (assign_managers != null)
            foreach (Action_assign_manager action in assign_managers.actions)
                action.apply_action(user_service, store_service, assign_managers);

        if (assign_owners != null)
            foreach (Action_assign_owner action in assign_owners.actions)
                action.apply_action(user_service, store_service, assign_owners);

        if (logout_users != null)
            foreach (Action_logout action in logout_users.actions)
                action.apply_action(user_service, store_service, logout_users);
    }
}
```

