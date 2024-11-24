# Bước 1: Build ứng dụng
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

RUN apk add --no-cache tzdata chromium \
    && ln -sf /usr/share/zoneinfo/Asia/Bangkok /etc/localtime \
    && echo "Asia/Bangkok" > /etc/timezone

# Copy toàn bộ mã nguồn vào container
COPY . /source
WORKDIR /source/API

ARG TARGETARCH

# Build ứng dụng .NET và publish vào /app
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# Copy file .db từ thư mục API vào thư mục publish /app/data
RUN mkdir -p /app/data && cp *.db /app/data/

# Bước 2: Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Cài đặt Chromium cho Puppeteer và các thư viện cần thiết
RUN apk add --no-cache chromium \
    && apk add --no-cache libx11 libxcomposite libxdamage libxtst


# Cấu hình đường dẫn đến Chromium và các tham số Puppeteer
ENV PUPPETEER_EXECUTABLE_PATH=/usr/bin/chromium-browser
ENV PUPPETEER_ARGS="--no-sandbox --disable-setuid-sandbox --headless"

# Copy ứng dụng đã publish từ build stage sang container cuối cùng
COPY --from=build /app .

# Đảm bảo thư mục /app/data có quyền ghi cho SQLite
RUN chmod -R 777 /app/data

ENTRYPOINT ["dotnet", "API.dll"]
