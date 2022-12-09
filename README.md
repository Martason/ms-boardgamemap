### Available Endpoints 
# BoardgameMap

## AuthService  
*Roll: Skapa nya konton och generera token via login.*    
POST /register  
POST /login 
  
## BoardgameReportsService  
*Roll: Att fungera som en gateway och skicka vidare http-requests från klienten till övriga mikrotjänster. Sammanställa data och validera token.*  
POST /register
POST /login
GET /allGames => hämtar alla boardgames.   
POST /eclipse/{town} => möjlighet att posta brädspelspartier i en stad  
POST /monopoly/{town} => möjlighet att posta brädspelspartier i en stad   
  
## MonopolyService   
GET /monopoly => hämtar alla spelade Monopolpartier.  
POST /monopoly/ {stad}  [Authorize] 
DELETE /monopoly/{id} => baserat på Boardgame-id [Authorize]  

## EclipseService   
GET /eclipse  => hämtar alla spelade Eclipsespartier.   
GET /eclipse/{id} => hämtar specifikt brädspesparti  
POST /eclipse/{stad} [Authorize]   
DELETE /eclipse/{id} 	=> baserat på Boardgame-id  [Authorize]     
