const http = require('http');
const sql = require('mssql');

const config = {
  user: 'sa', // Replace with your SQL Server username
  password: 'Dometrain#123', // Replace with your SQL Server password
  server: 'localhost', // Replace with your SQL Server hostname or IP
  database: 'podcasts',
  options: {
    encrypt: true, // Use encryption if required
    trustServerCertificate: true // Change to false if not using self-signed certificates
  }
};

const server = http.createServer(async (req, res) => {
  if (req.url === '/') {
    try {
      await sql.connect(config);
      const result = await sql.query('SELECT Title FROM Podcasts');
      const podcasts = result.recordset.map(row => row.Title);

      res.writeHead(200, { 'Content-Type': 'text/html' });
      res.write(`
        <!DOCTYPE html>
        <html>
        <head>
          <title>Podcasts</title>
          <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha3/dist/css/bootstrap.min.css" rel="stylesheet">
        </head>
        <body class="bg-light">
          <div class="container py-5">
            <h1 class="text-center mb-4">List of Podcasts</h1>
            <ul class="list-group">
              ${podcasts.map(podcast => `<li class="list-group-item">${podcast}</li>`).join('')}
            </ul>
          </div>
        </body>
        </html>
      `);
      res.end();
    } catch (err) {
      console.error('Database connection error:', err);
      res.writeHead(500, { 'Content-Type': 'text/plain' });
      res.end('Internal Server Error');
    }
  } else {
    res.writeHead(404, { 'Content-Type': 'text/plain' });
    res.end('404 Not Found');
  }
});

const port = process.env.PORT || 3000;
server.listen(port, () => {
  console.log(`Server is running on http://localhost:${port}`);
});