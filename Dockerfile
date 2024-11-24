# Build ứng dụng .NET và publish vào /app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
RUN apk add --no-cache tzdata \
    && ln -sf /usr/share/zoneinfo/Asia/Bangkok /etc/localtime \
    && echo "Asia/Bangkok" > /etc/timezone \
    && apk add --no-cache \
        chromium \
        nss \
        freetype \
        harfbuzz \
        ca-certificates \
        ttf-freefont

# Copy toàn bộ mã nguồn vào container
COPY . /source
WORKDIR /source/API

ARG TARGETARCH

# Build ứng dụng .NET và publish vào /app
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# Copy file .db từ thư mục API vào thư mục publish /app/data
RUN mkdir -p /app/data && cp *.db /app/data/

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Copy ứng dụng đã publish từ build stage sang container cuối cùng
COPY --from=build /app .

# Cài đặt Chromium vào final container
RUN apk add --no-cache chromium

# Đảm bảo thư mục /app/data có quyền ghi cho SQLite
RUN chmod -R 777 /app/data

ENTRYPOINT ["dotnet", "API.dll"]
