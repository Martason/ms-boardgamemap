### Available Endpoints 
# BoardgameMap

## AuthService  
*Roll: Skapa nya konton och generera token via login. Använder en egen databas*    
POST /register  
POST /login 
  
## BoardgameReportsService  
*Roll: Att fungera som en gateway och skicka vidare http-requests från klienten till övriga mikrotjänster. Sammanställa data från Eclipse och Monoplyservice samt validera token.*  
POST /register
POST /login
GET /allGames => hämtar alla boardgames.   
POST /eclipse/{town} => möjlighet att posta brädspelspartier i en stad. Kräver att man är inloggad  
POST /monopoly/{town} => möjlighet att posta brädspelspartier i en stad. Kräver att man är inloggad 

## MonopolyService
*Roll: Ansvarar för post- och getanrop för spelade partier av brädspelet Monopol. Använder en egen databas* 
GET /monopoly => hämtar alla spelade Monopolpartier.  
POST /monopoly/ {stad}  [Authorize]   
DELETE /monopoly/{id} => baserat på Boardgame-id [Authorize]  

## EclipseService
*Roll: Ansvarar för post- och getanrop för spelade partier av brädspelet Eclipse. Använder en egen databas*
GET /eclipse  => hämtar alla spelade Eclipsespartier.   
GET /eclipse/{id} => hämtar specifikt brädspesparti  
POST /eclipse/{stad} [Authorize]     
DELETE /eclipse/{id} 	=> baserat på Boardgame-id  [Authorize]     
