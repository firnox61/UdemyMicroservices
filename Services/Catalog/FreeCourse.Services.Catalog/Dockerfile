# 1. .NET 8 SDK'yı taban imaj olarak kullan
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# 2. Çalışma dizinini ayarla
WORKDIR /app

# 3. Portu aç (isteğe bağlı, runtime aşamasına taşınabilir)
EXPOSE 80

# 4. Proje dosyalarını kopyala
COPY Shared/FreeCourses.Shared/*.csproj Shared/FreeCourses.Shared/
COPY Services/Catalog/FreeCourse.Services.Catalog/*.csproj Services/Catalog/FreeCourse.Services.Catalog/

# 5. Bağımlılıkları geri yükle
RUN dotnet restore Services/Catalog/FreeCourse.Services.Catalog/*.csproj

# 6. Tüm dosyaları kopyala ve projeyi yayınla
COPY . .
RUN dotnet publish Services/Catalog/FreeCourse.Services.Catalog/*.csproj -c Release -o out

# 7. .NET Runtime imajını kullan
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# 8. Çalışma dizinini ayarla
WORKDIR /app

# 9. Yayınlanan dosyaları kopyala
COPY --from=build /app/out .

# 10. Uygulamayı çalıştır
ENTRYPOINT ["dotnet", "FreeCourse.Services.Catalog.dll"]
