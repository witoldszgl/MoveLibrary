<template>
  <div class="header-container py-4 mx-auto">
    <h1 class="text-center mb-4">Movie Library</h1>
    <div class="d-flex justify-content-between mb-3">
      <button class="btn btn-secondary" @click="$emit('show-add-modal')">Add Movie</button>
      <button class="btn btn-primary" @click="$emit('fetch-external-movies')">Fetch Movies</button>
    </div>
  </div>
</template>

<script>
export default {
  name: "HeaderComponent",
};
</script>

<style scoped>
.header-container {
  background-color: #343a40;
  color: white;
  padding: 20px;
  border-radius: 5px;
  width: 60%;
  margin: 0 auto; 
}

.btn-secondary{
  background-color: #10c228;
  color: white;
}
</style>
