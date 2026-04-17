module.exports = {
  apps: [
    {
      name: 'automation-hub',
      script: 'hub-server.js',
      cwd: __dirname,
      watch: false,
      autorestart: true,
      max_restarts: 10,
      restart_delay: 3000,
      env: {
        NODE_ENV: 'production'
      }
    },
    {
      name: 'kabal-scraper',
      script: 'kabal-scraper.js',
      cwd: __dirname + '/scraper-local',
      watch: false,
      autorestart: true,
      max_restarts: 10,
      restart_delay: 3000,
      env: {
        NODE_ENV: 'production'
      }
    },
    {
      name: 'morning-report',
      script: 'server.js',
      cwd: __dirname + '/morning-report',
      watch: false,
      autorestart: true,
      max_restarts: 10,
      restart_delay: 3000,
      env: {
        NODE_ENV: 'production'
      }
    }
  ]
};
