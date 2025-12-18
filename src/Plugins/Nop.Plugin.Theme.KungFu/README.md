# Kung Fu Theme Plugin

This plugin provides the Kung Fu martial arts store theme with automated synchronization of theme assets, topics, message templates, and AI-powered personalization features.

## Features

### 1. Theme Asset Management
- Automatic deployment of theme files from plugin directory to active theme directory
- Version-aware synchronization on plugin install/update
- Manual resync capability via admin configuration

### 2. Topic Management
- Automatically creates and updates store topics with branded content
- Includes topics: About Us, Contact Us, Shipping Info, Privacy Policy, and more
- HTML content stored in `/Themes/KungFu/Content/topics/`

### 3. Email Message Templates
The plugin provides professionally designed, branded email templates with:
- Kung Fu theme styling (teal/indigo gradient colors)
- Philosophical quotes from Lao Tzu and Confucius
- Responsive HTML design
- Templates include:
  - Order placed, completed, and cancelled notifications
  - Customer welcome messages
  - Email validation and password recovery
  - Shipment notifications
  - AI-generated sage messages (when configured)

All templates are stored in `/MessageTemplates/` and automatically synced on installation.

### 4. PDF Branding
- Custom footer text with branded messages and meditation quotes
- SVG logo support (no binary images)
- Automatically configured during plugin installation

### 5. Azure OpenAI Integration (Optional)
Generate personalized philosophical messages for customers after order payment using Azure OpenAI.

#### Configuration
1. Navigate to **Admin Panel → Configuration → Plugins → Theme.KungFu**
2. Scroll to "AI Sage Configuration" section
3. Configure the following settings:
   - **Azure OpenAI Endpoint**: Your Azure resource endpoint (e.g., `https://your-resource.openai.azure.com/`)
   - **Azure OpenAI API Key**: Your authentication key
   - **Deployment Name**: Your model deployment name (e.g., `gpt-4`, `gpt-35-turbo`)
   - **Enable AI Sage Messages**: Toggle to enable/disable the feature

#### How It Works
When enabled, the AI Sage service:
1. Monitors order payment completion events
2. Analyzes the order contents (products, quantities, categories)
3. Generates a personalized 3-4 sentence message inspired by Lao Tzu and Confucius
4. Relates the order to concepts of discipline, balance, and the path to mastery
5. Sends the message via the `ORDER_PAID_AI_SAGE_NOTIFICATION` email template

#### Fallback Behavior
If Azure OpenAI is not configured or fails, the service automatically uses predefined wisdom quotes to ensure customers always receive a thoughtful message.

## Installation

1. Install the plugin through NopCommerce admin panel
2. The plugin will automatically:
   - Copy theme files to the active themes directory
   - Create/update branded message templates
   - Configure PDF branding with meditation quotes
   - Set up topic pages

## Configuration

Navigate to **Admin Panel → Configuration → Plugins → Theme.KungFu** to:
- View synchronization status
- Enable/disable automatic sync on version changes
- Manually trigger a resync
- Configure Azure OpenAI settings

## Technical Details

### Message Template Tokens
All standard NopCommerce tokens are supported. The AI integration adds:
- `%AI.SageMessage%` - AI-generated or fallback wisdom message

### Event Consumers
- `OrderTokenEventConsumer` - Adds AI sage message tokens to order notifications

### Services
- `IThemeKungFuService` / `ThemeKungFuService` - Core theme synchronization
- `IAISageService` / `AISageService` - AI message generation with Azure OpenAI

### Dependencies
- Nop.Core
- Nop.Data
- Nop.Services
- Nop.Web.Framework
- System.Net.Http (for Azure OpenAI API calls)

## File Structure
```
Nop.Plugin.Theme.KungFu/
├── Assets/
│   └── kungfu-logo.svg           # SVG logo for branding
├── Controllers/
│   └── ThemeKungFuController.cs  # Admin configuration
├── Infrastructure/
│   ├── ThemeKungFuStartup.cs     # DI registration
│   ├── RouteProvider.cs
│   └── OrderPaidEventConsumer.cs # Order event handling
├── MessageTemplates/             # Email template HTML files
│   ├── order-placed-customer.html
│   ├── order-completed-customer.html
│   ├── order-cancelled-customer.html
│   ├── customer-welcome.html
│   ├── email-validation.html
│   ├── shipment-sent-customer.html
│   ├── password-recovery.html
│   └── order-paid-ai-sage.html
├── Models/
│   └── ConfigurationModel.cs
├── Services/
│   ├── IThemeKungFuService.cs
│   ├── ThemeKungFuService.cs
│   ├── IAISageService.cs
│   └── AISageService.cs
├── Themes/
│   └── KungFu/                   # Theme assets
├── Views/
│   └── Configure.cshtml          # Admin configuration UI
└── ThemeKungFuPlugin.cs          # Plugin entry point
```

## Security Notes
- API keys are stored encrypted in settings
- No binary images are committed (SVG only)
- Error handling prevents API failures from breaking order processing
- Fallback messages ensure graceful degradation

## Future Enhancements
- Additional language support for international customers
- More AI-powered personalization options
- Extended template customization UI
- Analytics for message effectiveness

## License
See main NopCommerce solution license.
