# 🎯 Frontend Dockerfile
FROM node:20-alpine AS build

WORKDIR /app

# Copy package files
COPY FlightInfo.Frontend/package*.json ./
RUN npm install

# Copy source code
COPY FlightInfo.Frontend/ .

# Clean install to fix rollup issues
RUN rm -rf node_modules package-lock.json
RUN npm install

# Build the app
RUN npm run build

# Production stage
FROM nginx:alpine

# Copy built app to nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Copy nginx config
COPY FlightInfo.Frontend/nginx.conf /etc/nginx/nginx.conf

EXPOSE 5173

CMD ["nginx", "-g", "daemon off;"]
