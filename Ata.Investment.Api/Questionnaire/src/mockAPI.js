// in case want to use webpack's own built in server for mock API.
// But it seems to hardcode all data

export default function setupMockRoutes(app) {
  app.get('/users', function(req, res) {
    // Hard coding for simplicity. Pretend this hits a real database
    res.json([
      {"id": 1,"firstName":"Bob","lastName":"Smith","email":"bob@gmail.com"},
      {"id": 2,"firstName":"Tammy","lastName":"Norton","email":"tnorton@yahoo.com"},
      {"id": 3,"firstName":"Tina","lastName":"Lee","email":"lee.tina@hotmail.com"}
    ]);
  });
}
