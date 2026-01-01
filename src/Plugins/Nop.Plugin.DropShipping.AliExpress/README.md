# AliExpress Drop Shipping Plugin for NopCommerce

A comprehensive NopCommerce plugin that automates drop shipping with AliExpress integration. This plugin enables seamless product import, price synchronization, order management, and automated fulfillment workflows.

## Features

### 🔐 Authentication & Configuration
- **OAuth 2.0 Integration**: Secure authentication with AliExpress API
- **Automatic Token Refresh**: Background task ensures tokens stay valid
- **Flexible Configuration**: Comprehensive settings for pricing, shipping, and automation

### 🛍️ Product Management
- **Product Search**: Search AliExpress products directly from the admin panel
- **Easy Product Linking**: Link NopCommerce products to AliExpress items with a single click
- **Automated Price Calculation**: 
  - Base product price from AliExpress
  - Shipping costs
  - VAT (configurable %)
  - Customs duty (configurable %, default 10%)
  - Profit margin (configurable %)
- **Image Import**: Automatically download and store product images
- **Attribute Mapping**: Map AliExpress product attributes to NopCommerce
- **Specification Sync**: Import product specifications

### 🔄 Synchronization
- **Daily Price Sync**: Automated task updates prices and availability
- **Conflict Resolution**: Smart handling of price discrepancies
- **Sync Status Tracking**: Monitor last sync time and status for each product
- **Availability Monitoring**: Automatically mark products unavailable when out of stock on AliExpress

### 📦 Order Management
- **Automatic Order Creation**: Orders placed in your store automatically create orders on AliExpress
- **Order Tracking**: Real-time tracking of AliExpress shipments
- **Status Webhooks**: Receive delivery notifications from AliExpress
- **Order Status Workflow**:
  1. Customer places order → Auto-create on AliExpress
  2. AliExpress ships → Update tracking info
  3. AliExpress delivers → Move to "Processing" status
  4. Auto-create local shipment with Courier Guy
  5. Update to "Ready to Ship"

### 🚚 Shipping Integration
- **Courier Guy Integration**: Seamless integration with Courier Guy plugin
- **Automated Fulfillment**: Create local shipments when AliExpress marks order as delivered
- **Shipment Filtering**: Automatically filter shipments for AliExpress items

### 🎨 User Interface
- **Material Design**: Beautiful UI with Google Material Icons
- **NopCommerce Compliant**: Follows NopCommerce design patterns
- **Responsive Layout**: Works perfectly on all screen sizes
- **Search Modal**: Elegant product search with preview images
- **Configuration Dashboard**: Comprehensive settings page with visual status indicators

## Installation

1. **Download the Plugin**
   - Clone or download this repository
   - Build the solution in Release mode

2. **Deploy to NopCommerce**
   - Copy the compiled plugin to `Presentation/Nop.Web/Plugins/DropShipping.AliExpress/`
   - Restart the application

3. **Install via Admin Panel**
   - Navigate to Configuration → Local Plugins
   - Find "AliExpress Drop Shipping" in the list
   - Click "Install"
   - Restart when prompted

## Configuration

### 1. Get AliExpress API Credentials

1. Register as a developer at [AliExpress Open Platform](https://openservice.aliexpress.com/)
2. Create a new application
3. Note your App Key and App Secret

### 2. Configure the Plugin

1. Go to Configuration → Local Plugins → AliExpress Drop Shipping
2. Click "Configure"
3. Enter your App Key and App Secret
4. Click "Save"

### 3. Authorize the Application

1. On the configuration page, click "Authorize with AliExpress"
2. Sign in to your AliExpress developer account
3. Approve the authorization
4. Copy the authorization code from the redirect URL
5. Paste it in the "Authorization Code" field
6. Click "Exchange Code for Token"

### 4. Configure Pricing

Set your pricing parameters:
- **Default Margin %**: Your profit margin (e.g., 25%)
- **VAT %**: Value-added tax percentage (e.g., 15%)
- **Customs Duty %**: Import duty percentage (typically 10%)
- **Default Currency**: Your store's currency (e.g., ZAR, USD)
- **Default Shipping Country**: Country code for shipping calculations (e.g., ZA)

### 5. Configure Automation

- **Enable Daily Sync**: Automatically sync product prices daily
- **Daily Sync Hour**: Hour (0-23) to run the sync
- **Auto Create Orders**: Automatically place orders on AliExpress
- **Auto Create Local Shipments**: Create Courier Guy shipments on AliExpress delivery

## Usage

### Adding AliExpress Products

1. **Create/Edit a Product**
   - Go to Catalog → Products
   - Create a new product or edit an existing one

2. **Search AliExpress**
   - Scroll to the "AliExpress Product Selection" section
   - Click "Search AliExpress Products"
   - Enter a search keyword
   - Click "Search"

3. **Select a Product**
   - Browse the search results
   - Click on a product to select it
   - The product will be linked to your NopCommerce product

4. **Save**
   - Click "Save" to save the product
   - The price will be automatically calculated based on your settings

### Managing Orders

Orders are managed automatically:

1. **Customer Places Order**
   - Order is created in NopCommerce
   - Plugin automatically creates order on AliExpress (if enabled)

2. **Track Shipment**
   - Hourly task checks for updates
   - Tracking information is updated automatically

3. **Delivery Processing**
   - When AliExpress marks order as delivered
   - Order status changes to "Processing"
   - Local shipment is created with Courier Guy (if enabled)

4. **Local Delivery**
   - Courier Guy handles local shipping
   - Customer receives package

### Webhook Configuration

To receive real-time updates from AliExpress:

1. Configure webhook URL in AliExpress developer console:
   - Order Status: `https://yourstore.com/Plugins/AliExpressWebhook/OrderStatus`
   - Tracking Updates: `https://yourstore.com/Plugins/AliExpressWebhook/Tracking`

2. Ensure your server can receive POST requests at these endpoints

## Scheduled Tasks

The plugin creates three scheduled tasks:

### 1. Token Refresh Task
- **Frequency**: Daily
- **Purpose**: Refreshes access token before expiry
- **Configuration**: Set days before expiry in plugin settings

### 2. Product Sync Task
- **Frequency**: Daily (configurable hour)
- **Purpose**: Updates prices and availability
- **Configuration**: Enable/disable and set hour in plugin settings

### 3. Order Tracking Task
- **Frequency**: Hourly
- **Purpose**: Polls AliExpress for order status updates
- **Note**: Complements webhook system for reliability

## Price Calculation Formula

The plugin calculates the final selling price using this formula:

```
Base Amount = Product Price + Shipping Cost
Customs Duty = Base Amount × (Customs Duty % ÷ 100)
Subtotal = Base Amount + Customs Duty
VAT = Subtotal × (VAT % ÷ 100)
Total Before Margin = Subtotal + VAT
Final Price = Total Before Margin × (1 + Margin % ÷ 100)
```

### Example

For a product with:
- AliExpress Price: $20
- Shipping: $5
- VAT: 15%
- Customs: 10%
- Margin: 25%

```
Base Amount = $20 + $5 = $25
Customs Duty = $25 × 0.10 = $2.50
Subtotal = $25 + $2.50 = $27.50
VAT = $27.50 × 0.15 = $4.13
Total Before Margin = $27.50 + $4.13 = $31.63
Final Price = $31.63 × 1.25 = $39.54
```

## Troubleshooting

### Token Issues

**Problem**: "Access token is invalid or expired"

**Solution**:
1. Check if token has expired in configuration page
2. Click "Refresh Token Now" button
3. If that fails, go through authorization flow again

### Sync Issues

**Problem**: Products not syncing

**Solution**:
1. Check if "Enable Daily Sync" is enabled
2. Verify the token is valid
3. Check scheduled task is enabled and running
4. Review logs for error messages

### Order Creation Fails

**Problem**: Orders not created on AliExpress

**Solution**:
1. Verify "Auto Create Orders" is enabled
2. Check product has valid AliExpress mapping
3. Ensure customer shipping address is complete
4. Review error messages in AliExpress orders table

## Database Schema

### AliExpressProductMapping
Stores product mapping data including prices, shipping, and sync status.

### AliExpressOrder
Stores order tracking data and links NopCommerce orders to AliExpress orders.

## Support

For issues and questions:
- Check the logs in System → Log
- Review scheduled task execution history
- Ensure all prerequisites are met
- Verify API credentials are correct

## License

This plugin is released under the MIT License.

## Credits

Built with ❤️ using:
- NopCommerce 4.90
- AliExpress Open Platform API
- Material Design Icons
- .NET 9.0

## Version History

### v1.0.0
- Initial release
- Full drop shipping automation
- Product search and linking
- Automated order creation
- Price synchronization
- Order tracking
- Courier Guy integration
- Material Design UI
