# syntax=docker/dockerfile:1

# Build stage
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Cài đặt timezone và Chromium cùng các thư viện phụ thuộc
RUN apk add --no-cache tzdata \
    && ln -sf /usr/share/zoneinfo/Asia/Bangkok /etc/localtime \
    && echo "Asia/Bangkok" > /etc/timezone

# Cài đặt các thư viện cần thiết cho Chromium và Puppeteer
RUN apk add --no-cache \
    chromium \
    nss \
    freetype \
    harfbuzz \
    ca-certificates \
    ttf-freefont \
    libstdc++ \
    libx11 \
    libxcomposite \
    libxrandr \
    libxdamage \
    libxext \
    && apk add --no-cache --virtual .build-deps \
    curl \
    && apk del .build-deps

# Đặt đường dẫn Chromium cho Puppeteer
ENV PUPPETEER_SKIP_CHROMIUM_DOWNLOAD=true
ENV PUPPETEER_EXECUTABLE_PATH=/usr/bin/chromium-browser

# Đảm bảo quyền thực thi cho Chromium
RUN chmod +x /usr/bin/chromium-browser

# Copy toàn bộ mã nguồn vào container
COPY . /source
WORKDIR /source/API

# Xây dựng ứng dụng .NET và publish vào /app
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# Copy file .db từ thư mục API vào thư mục publish /app/data
RUN mkdir -p /app/data && cp *.db /app/data/

# Final stage (runtime environment)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Copy ứng dụng đã build từ build stage sang container cuối cùng
COPY --from=build /app .

# Đảm bảo thư mục /app/data có quyền ghi cho SQLite
RUN chmod -R 777 /app/data

# Mở cổng 8080 cho ứng dụng
EXPOSE 8080

# Chạy ứng dụng .NET trên cổng 8080
ENTRYPOINT ["dotnet", "API.dll"]
