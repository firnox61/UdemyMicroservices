# 1. .NET 8 SDK'yı taban imaj olarak kullan
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

# 2. Çalışma dizinini ayarla
WORKDIR /app

# 3. Portu aç (isteğe bağlı, runtime aşamasına taşınabilir)
EXPOSE 80

# 4. Proje dosyalarını kopyala
COPY Shared/FreeCourses.Shared/*.csproj Shared/FreeCourses.Shared/
COPY IdentityServer/FreeCourse.IdentityServer/*.csproj IdentityServer/FreeCourse.IdentityServer/

# 5. Bağımlılıkları geri yükle
RUN dotnet restore IdentityServer/FreeCourse.IdentityServer/*.csproj

# 6. Tüm dosyaları kopyala ve projeyi yayınla
COPY . .
RUN dotnet publish IdentityServer/FreeCourse.IdentityServer/*.csproj -c Release -o out

# 7. .NET Runtime imajını kullan
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime

# 8. Çalışma dizinini ayarla
WORKDIR /app

# 9. Yayınlanan dosyaları kopyala
COPY --from=build /app/out .

# 10. Uygulamayı çalıştır
ENTRYPOINT ["dotnet", "FreeCourse.IdentityServer.dll"]
