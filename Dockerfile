FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

RUN apk add --no-cache tzdata \
    && ln -sf /usr/share/zoneinfo/Asia/Bangkok /etc/localtime \
    && echo "Asia/Bangkok" > /etc/timezone

RUN apk add --no-cache \
    chromium \
    nss \
    freetype \
    harfbuzz \
    ca-certificates \
    ttf-freefont \
    libx11 \
    libxcomposite \
    libxrandr \
    libgdk-pixbuf \
    libatk-bridge2.0-0 \
    libatk1.0-0 \
    libcups \
    && apk add --no-cache --virtual .build-deps \
    curl \
    && apk del .build-deps

# Đặt đường dẫn Chromium cho Puppeteer
ENV PUPPETEER_SKIP_CHROMIUM_DOWNLOAD=true
ENV PUPPETEER_EXECUTABLE_PATH=/usr/bin/chromium-browser

# Copy toàn bộ mã nguồn vào container
COPY . /source
WORKDIR /source/API
