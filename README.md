Plan projekt 
BoardgameMap

AuthService
Roll: Skapa nya konton och generera token via login.
POST /register
POST /login
BoardgameReportsService
Roll: Att sammanställa data (bland annat till frontend och emailservice) genom att kommunicera med olika Boardgame-Api:er, exempelvis SettlersService. 
GET / {stad} - hämtar alla boardgames som har spelats i den staden. 
GET / {boardgame} - hämtar var i Sverige som man har spelat det brädspelet. 
GET / {user} - hämtar vilka boardgames denna user spelat och var i Sverige

MonopolyService 
GET / hämtar alla spelade Monopolpartier.
POST  / {user} / {stad} 
DELETE  /{id}	 => baserat på Boardgame-id

SettlersService 
GET /boardgames  => hämtar alla spelade Settlerspartier.
POST /boardgame/{user} / {stad} 
DELETE /boardgame/{id} 	=> baserat på Boardgame-id

EmailService 
Roll: Ansvar för att skicka ut önskad rapport till valfri mail. Kommunicerar med BoardgameReportsService för att hämta rapporter. 
GET / {email}/{rapportmodel}
POST / {email}/{rapportmodel}
