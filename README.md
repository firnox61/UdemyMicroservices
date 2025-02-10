# Proje Adı

Bu proje, mikroservis tabanlı bir mimari kullanarak çeşitli hizmetlerin yönetilmesini sağlayan bir sistemdir. Kullanıcılar mobil veya web üzerinden sisteme erişebilir ve çeşitli API'ler aracılığıyla işlemlerini gerçekleştirebilir.

## Genel Mimari

### Clients

- **Mobile Clients**: Mobil cihazlar üzerinden erişim.
- **Web Clients**: Web tarayıcıları üzerinden erişim.

### API Gateway

- **API Gateway (Web)**: Web istemcileri için API isteklerini yöneten geçit.
- **API Gateway (Mobile)**: Mobil istemcileri için API isteklerini yöneten geçit.
- **BFF (Backend for Frontend)**: API Gateway'in bir parçası olarak arka uç ile ön uç arasındaki iletişimi yönetir.

### Mikroservisler

- **Catalog API**: Katalog verileri ile ilgili işlemleri yöneten API.
- **Basket API**: Sepet yönetimi işlemlerini yöneten API.
- **Discount API**: İndirimlerle ilgili işlemleri yöneten API.
- **Photo Stock API**: Fotoğraf stoğu ile ilgili işlemleri yöneten API.
- **Order API**: Sipariş işlemlerini yöneten API.

### Veri Yönetimi

- **SQLServer**: İlişkisel veritabanı, temel veri yönetimi için kullanılır.
- **Redis**: Cache mekanizması olarak kullanılır.
- **PostgreSQL**: Discount API veritabanı olarak kullanılır.
- **MongoDB**: MongoDB driver üzerinden bağlantı sağlar, NoSQL veritabanı.
- **RabbitMQ (Message Broker)**: Mesajlaşma aracı olarak mikroservisler arası iletişimi sağlar.


### Bağlantı ve İletişim

- **API Gateway - Basket API**: Mobil ve web istemcilerinden gelen istekler API Gateway üzerinden Basket API'ye yönlendirilir.
- **API Gateway - Catalog API**: Mobil ve web istemcilerinden gelen istekler API Gateway üzerinden Catalog API'ye yönlendirilir.
- **Basket API - Redis**: Sepet verilerini hızlı bir şekilde yönetmek için Redis kullanılır.
- **Discount API - PostgreSQL**: İndirim verilerini yönetmek için PostgreSQL kullanılır.
- **Order API - RabbitMQ**: Sipariş işlemlerinde mesajlaşma için RabbitMQ kullanılır.
- **Photo Stock API - MongoDB**: Fotoğraf verilerini yönetmek için MongoDB kullanılır.
- **ElasticSearch - RabbitMQ**: Loglama ve analiz işlemlerinde ElasticSearch ve RabbitMQ kullanılır.

### Kullanıcı Etkileşimi

- **Clients -> API Gateway (Mobile/Web)**: Kullanıcılar mobil veya web üzerinden API Gateway'e istek gönderir.
- **API Gateway -> Mikroservisler**: İstekler ilgili mikroservislere yönlendirilir.
- **Mikroservisler -> Veri Depoları**: Mikroservisler, ilgili veri depoları ve cache mekanizmaları ile iletişim kurar.
- **RabbitMQ**: Mikroservisler arasında mesajlaşma ve iş süreçlerinin yönetimi için kullanılır.

Bu mimari, mikroservis tabanlı bir sistemde kullanıcı isteklerini yönetmek, veri depolamak ve iş süreçlerini etkin bir şekilde yönetmek için tasarlanmıştır.
![Ekran görüntüsü 2025-02-10 143916](https://github.com/user-attachments/assets/cff74ba6-7d73-4566-9a7f-80ba571ea1fc)
![Ekran görüntüsü 2025-02-10 150522](https://github.com/user-attachments/assets/ae43d287-c5d1-4d6e-a03c-d38844d049b7)
![Ekran görüntüsü 2025-02-10 155125](https://github.com/user-attachments/assets/19810560-acfc-4a9e-8ef7-05d4326488aa)
![Ekran görüntüsü 2025-02-10 155708](https://github.com/user-attachments/assets/74f1c719-55d7-4816-ab69-387aacbfe21d)
![Ekran görüntüsü 2025-02-10 155648](https://github.com/user-attachments/assets/40fb16d0-8107-4780-beee-5819c3361314)
![Ekran görüntüsü 2025-02-10 155630](https://github.com/user-attachments/assets/51fa0eb1-c850-4e30-9e83-b7ea1a44dbdc)
![image](https://github.com/user-attachments/assets/634514ae-b3d9-4e11-bcae-fde4d242d655)

