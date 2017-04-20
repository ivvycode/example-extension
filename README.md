# Example Extension
An example project that leverages AWS technologies to extend the iVvy platform.
# Setup
1. Sign up for an AWS account (https://aws.amazon.com/)
2. Create an IAM administrator user with api credentials - store the credentials in a secure place.
3. Install the AWS cli tool (https://aws.amazon.com/cli/)
4. Configure an _exampleExtensionDevelopment_ profile with the credentials created earlier.
    ```
    [exampleExtensionDevelopment]
    region=ap-southeast-2
    aws_access_key_id=YOUR KEY
    aws_secret_access_key=YOUR SECRET
    ```
5. Install dotnet core (https://www.microsoft.com/net/core)
6. Install serverless framework (https://serverless.com/)
7. In the _ExampleExtension_ source directory, create a _secrets.yml_ file from the _secrets.example.yml_ file.
NOTE: Generate **new** secrets. Do **not** use the secrets in the example file. Secrets can be generated with the following c# code:
    ```
    byte[] secretBytes = new byte[32];
    RandomNumberGenerator.Create().GetBytes(secretBytes);
    string secret = Convert.ToBase64String(secretBytes);
    ```
8. Build the extension.
    ```
    cd src/ExampleExtension/
    ./build.sh
    ```
9. Deploy the extension to your AWS account.
    ```
    cd src/ExampleExtension/
    sls deploy
    ```
10. Notify iVvy of your extension details (including setup, unsetup & configure endpoints).

