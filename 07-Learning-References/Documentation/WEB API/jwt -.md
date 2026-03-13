jwt -
authentication is usernaem and password

authorization is will decide this user has access to this section

json web token

used between in web apis to claim the data or access info from the server

uses HTTP stateless - so server requires all the info about the client to  process the data

it is a self contained token

it is encoded in strin g format

this token is used to authenticate and authorize the users

problem without jwt token , the server needs to remember and the store the session data ,not scalable

in authorization we can add policies to check the endpoint
Policies are just rules over claims.

•	Secret key (SecretKey)
Used to sign tokens and to verify they weren’t tampered with. Without it, the server cannot trust incoming JWTs.
•	Issuer (Issuer)
Identifies who created the token. Validation ensures the token came from your own auth system, not a different issuer.
•	Audience (Audience)
Identifies who the token is intended for. Validation ensures the token was issued for this API, not another app.
