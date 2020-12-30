# Two Factor Authentication Starter App

This is an elementary  login authentication use case of two-factor authentication via SMS. The main focus of this application is to understand and implement the 2FA flow, so least amount of stress is given to the authentication/login mechanism.

## Requirement

To run this starter app, it is recommended to install [Visual Studio](https://visualstudio.microsoft.com/vs/) and open the examples folder in Visual Studio.

## Installation
1. Copy `.env.example` and rename to `.env` and add the appropriate values. Check `Configuration` section for more details.
2. The Visual Studio would install the dependencies automatically.
3. Select the application that is to be run - 2fa and press the start/play button.

## Configuration
There are a few environment variables (check `.env` file) to make the application simpler and help us focus on the key aspects a two-factor authentication system via SMS. Some of the variables are pre-filled and some are left blank which are left on the user to place appropriate values. All the variables are mandatory.

ENV KEY           | Description
----------------- | -------------
CLIENT_ID         | Private project key
CLIENT_SECRET     | Private project secret
BASE_URL          | URL of the CPaaS server to use
PHONE_NUMBER      | Phone number that would receive the verification code
EMAIL             | Email used in the login screen of the application
PASSWORD          | Password to be entered against the EMAIL provided
DESTINATION_EMAIL | Email that would receive the verification code


## Usage
The application comprises of three simple pages, login, code verification, dashboard/portal
> + On opening the application in the browser, the login screen is presented. The user needs to enter the `Email` / `Password` that are specified in the `.env` file and click on the `Login` button.
> + Once the credentials are verified, the verification page is presented to user. Here the user has 2 options, either receive 2FA via SMS or via EMAIL. This phone number/email corresponds to the one entered in the .env file as PHONE_NUMBER/DESTINATION_EMAIL.
> + The user now needs to enter the verification code received in the mentioned phone number and click `Verify` button.
> + The application verifies the entered code. If the code validates, the user is redirected to the dashboard section; else the user will be promoted with an error alert `Code invalid or expired` and is required to re-enter the verification code.
> + As the user is authenticated, the dashboard opens up. The user can logout from the dashboard and login screen would be presented.
