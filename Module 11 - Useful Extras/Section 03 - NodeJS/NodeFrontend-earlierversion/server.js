const express = require('express');
const sql = require('mssql');
const app = express();

const port = process.env['PORT'] ?? 8080;

// SQL Server configuration
const dbConfig = {
    user: 'sa', // Replace with your SQL Server username
    password: 'Dometrain#123', // Replace with your SQL Server password
    server: 'localhost',
    database: 'podcasts',
    options: {
        encrypt: false, // Disable encryption for local development
        trustServerCertificate: true
    }
};

// Connect to the database
sql.connect(dbConfig).then(pool => {
    console.log('Connected to SQL Server');

    // API endpoint to fetch podcasts
    app.get('/api/podcasts', async (req, res) => {
        try {
            const result = await pool.request().query('SELECT Title FROM Podcasts');
            res.json(result.recordset);
        } catch (err) {
            console.error('Error querying database:', err);
            res.status(500).send('Error querying database');
        }
    });

}).catch(err => {
    console.error('Database connection failed:', err);
});

// Serve the main webpage
app.get('/', (req, res) => {
    res.sendFile(__dirname + '/index.html');
});

// Start the server
app.listen(port, () => {
    console.log(`Server is running on http://localhost:${port}`);
});