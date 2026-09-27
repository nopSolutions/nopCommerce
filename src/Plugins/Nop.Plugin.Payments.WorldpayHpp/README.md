# Worldpay Hosted Payment Pages (HPP) Plugin

This plugin integrates your application with **Worldpay Access Hosted Payment Pages (HPP)**, providing a secure, low-code checkout experience at the lowest PCI compliance level (SAQ A).

## 💳 Integration Method
The plugin strictly utilizes the **Full Page Redirect** model. 
* When a customer initiates a transaction, the plugin sends a payload to the Worldpay API to [Setup a Payment](https://docs.worldpay.com/access/products/hosted-payment-pages/setup-a-payment).
* Worldpay generates a secure transaction URL in the response.
* The customer is automatically **redirected away** from your website to the brand-customized Worldpay Hosted Payment Page to securely input their payment details.
* For a comprehensive structural overview of this product, refer to the [Worldpay Hosted Payment Pages Documentation](https://docs.worldpay.com/access/products/hosted-payment-pages).

## 🔔 Webhooks & Instant Payment Notifications
To guarantee order statuses are accurately updated even if a customer closes their browser before returning to your site, this plugin relies heavily on asynchronous **Webhooks** sent from Worldpay.

⚠️ **Important Whitelisting Requirement:**
To receive transaction notifications successfully:
1. Log in to your **Worldpay Merchant Portal**.
2. Locate the Webhooks / Notifications configuration area.
3. **Whitelist your store's specific endpoint URLs** (e.g., `https://yourstore.com`) within the portal so Worldpay's servers are allowed to push transaction updates to your environment.

## 🛠️ Configuration Checklist
* **API Credentials:** Ensure you have your base64-encoded Basic Auth credentials (Username/Password or Token) extracted from your Worldpay Dashboard.
* **Merchant Entity ID:** You must configure your exact merchant entity reference (found under "Developer Tools" in the dashboard).
* **Currency Support:** Verify that the currencies offered in your store settings match valid 3-character ISO codes supported by your Worldpay processing arrangement.