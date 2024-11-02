<template>
  <div class="page-container">
    <HeaderComponent
      @show-add-modal="showAddModal"
      @fetch-external-movies="fetchExternalMovies"
    />
    <MoviesTableComponent
      :movies="movies"
      @edit-movie="showEditModal"
      @delete-movie="deleteMovie"
    />
    <MovieModalComponent
      v-if="isModalVisible"
      :isEditing="isEditing"
      :movie="currentMovie"
      @close-modal="closeModal"
      @save-movie="saveMovie"
    />
  </div>
</template>

<script>
import axios from 'axios';
import HeaderComponent from './HeaderComponent.vue';
import MoviesTableComponent from './MoviesTableComponent.vue';
import MovieModalComponent from './MovieModalComponent.vue';

export default {
  name: "MoviesList",
  components: {
    HeaderComponent,
    MoviesTableComponent,
    MovieModalComponent,
  },
  data() {
    return {
      movies: [],
      isModalVisible: false,
      isEditing: false,
      currentMovie: {
        id: null,
        title: '',
        director: '',
        year: 1900,
        rate: 0.0,
      },
    };
  },
  methods: {
    async fetchMovies() {
      try {
        const response = await axios.get('http://localhost:5003/api/movie');
        this.movies = response.data;
      } catch (error) {
        console.error('Error fetching movies:', error);
        alert(`Error fetching movies: ${error.message}`);
      }
    },
    showAddModal() {
      this.isEditing = false;
      this.currentMovie = {
        id: null,
        title: '',
        director: '',
        year: 1900,
        rate: 0.0,
      };
      this.isModalVisible = true;
    },
    showEditModal(movie) {
      this.isEditing = true;
      this.currentMovie = { ...movie };
      this.isModalVisible = true;
    },
    async saveMovie(movie) {
      try {
        if (this.isEditing) {
          await axios.put(`http://localhost:5003/api/movie/${movie.id}`, movie);
        } else {
          await axios.post('http://localhost:5003/api/movie', movie);
        }

        this.fetchMovies();
        this.closeModal();
      } catch (error) {
        console.error('Error saving movie:', error);
      }
    },
    async deleteMovie(id) {
      if (confirm('Are you sure you want to delete this movie?')) {
        try {
          await axios.delete(`http://localhost:5003/api/movie/${id}`);
          this.fetchMovies();
        } catch (error) {
          console.error('Error deleting movie:', error);
        }
      }
    },
    closeModal() {
      this.isModalVisible = false;
    },
    async fetchExternalMovies() {
      try {
        await axios.get('http://localhost:5003/api/movie/fetch-external-movies');
        this.fetchMovies();
      } catch (error) {
        console.error('Error fetching movies:', error);
      }
    },
  },
  mounted() {
    this.fetchMovies();
  },
};
</script>

<style scoped>
.page-container {
  background: linear-gradient(to right, #d3d3d3, #f8f9fa);
  padding: 20px;
  min-height: 100vh;
}
</style>
