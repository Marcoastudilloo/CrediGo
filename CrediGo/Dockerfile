# Imagen base con .NET SDK 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Instala herramientas y dependencias para compilar Leptonica y Tesseract
RUN apt-get update && apt-get install -y --no-install-recommends \
    wget git build-essential autoconf automake libtool pkg-config \
    libpng-dev libjpeg-dev libtiff-dev zlib1g-dev libicu-dev \
    libpango1.0-dev libcairo2-dev libleptonica-dev ca-certificates && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Compila e instala Leptonica 1.82.0
WORKDIR /leptonica
RUN git clone --branch 1.82.0 https://github.com/DanBloomberg/leptonica.git . && \
    ./autogen.sh && ./configure && make && make install

# Compila e instala Tesseract 5.3.3
WORKDIR /tesseract
RUN git clone --branch 5.3.3 https://github.com/tesseract-ocr/tesseract.git . && \
    ./autogen.sh && ./configure && make && make install

# Configura ldconfig para buscar librerías en /usr/local/lib
RUN echo "/usr/local/lib" > /etc/ld.so.conf.d/usrlocal.conf && ldconfig

# Crea el enlace simbólico esperado por el runtime
RUN ln -sf /usr/local/lib/libleptonica.so.5 /usr/local/lib/libleptonica-1.82.0.so

# Descarga el archivo de idioma español para Tesseract
RUN wget https://github.com/tesseract-ocr/tessdata/raw/main/spa.traineddata -P /usr/local/share/tessdata

# Variables de entorno para Tesseract y el linker dinámico
ENV TESSDATA_PREFIX=/usr/local/share/
ENV LD_LIBRARY_PATH=/usr/local/lib:$LD_LIBRARY_PATH

# Copia el proyecto y publica en modo Release
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

# Imagen final con runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Copia las librerías de Tesseract y Leptonica
COPY --from=build /usr/local/lib /usr/local/lib
COPY --from=build /usr/local/share /usr/local/share

# Configura ldconfig para runtime
RUN echo "/usr/local/lib" > /etc/ld.so.conf.d/usrlocal.conf && ldconfig

# Establece variables de entorno para runtime
ENV TESSDATA_PREFIX=/usr/local/share/
ENV LD_LIBRARY_PATH=/usr/local/lib:$LD_LIBRARY_PATH

# Copia la publicación desde build
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "CrediGo.dll"]
