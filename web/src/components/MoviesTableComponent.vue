<template>
  <div class="table-container py-4 mx-auto">
    <div v-if="movies.length > 0">
      <table class="table table-striped">
        <thead class="thead-dark">
          <tr>
            <th>ID</th>
            <th>Title</th>
            <th>Director</th>
            <th>Year</th>
            <th>Rating</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="movie in movies" :key="movie.id">
            <td>{{ movie.id }}</td>
            <td>{{ movie.title }}</td>
            <td>{{ movie.director }}</td>
            <td>{{ movie.year }}</td>
            <td>{{ movie.rate }}</td>
            <td>
              <button class="btn btn-warning btn-sm me-2" @click="$emit('edit-movie', movie)">Edit</button>
              <button class="btn btn-danger btn-sm" @click="$emit('delete-movie', movie.id)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <p v-else class="text-center">No movies available.</p>
  </div>
</template>

<script>
export default {
  name: "MoviesTableComponent",
  props: {
    movies: Array,
  },
};
</script>

<style scoped>
.table-container {
  background-color: #f2f2f2;
  padding: 20px;
  width: 60%;
  margin: 20px auto; 
  border-radius: 5px;
  box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
}
.table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}
th,
td {
  border: 1px solid #ddd;
  padding: 8px;
}
th {
  background-color: #f2f2f2;
}
</style>
