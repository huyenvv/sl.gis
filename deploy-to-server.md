# SSH to server
ssh your_username@host_ip_address

wget https://packages.microsoft.com/config/ubuntu/20.10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
 
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-3.1

Check version .NET is really installed.
dotnet --version

sudo nano /etc/nginx/sites-available/default

server {
    listen        80;
    server_name   example.com *.example.com;
	listen 443 ssl; # managed by Certbot
	
	# RSA certificate
    ssl_certificate /etc/letsencrypt/live/example.com/fullchain.pem; # managed by Certbot
    ssl_certificate_key /etc/letsencrypt/live/example.com/privkey.pem; # managed by Certbot
	include /etc/letsencrypt/options-ssl-nginx.conf; # managed by Certbot
	
    location / {
        proxy_pass         http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}

sudo nano /etc/systemd/system/gissl.service

[Unit]
Description=Example .NET Web API App running on Ubuntu

[Service]
WorkingDirectory=/var/www/gissl
ExecStart=/usr/bin/dotnet /var/www/gissl/SLGIS.Web.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-example
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target

sudo systemctl enable gissl.service
sudo systemctl start gissl.service
sudo systemctl status gissl.service


sudo service gissl restart
sudo service nginx restart

# Setup SSL on Nginx with Let's encrypt (auto renew)
https://www.nginx.com/blog/using-free-ssltls-certificates-from-lets-encrypt-with-nginx/