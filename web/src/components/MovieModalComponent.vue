<template>
  <div class="modal fade show" style="display: block; background: rgba(0, 0, 0, 0.5);">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" style="color: black;">{{ isEditing ? 'Edit Movie' : 'Add Movie' }}</h5>
          <button type="button" class="btn-close" @click="closeModal"></button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="onSave">
            <div class="mb-3">
              <label for="title" class="form-label">Title:</label>
              <input
                type="text"
                v-model="$v.localMovie.title.$model"
                name="title"
                id="title"
                maxlength="200"
                @blur="$v.localMovie.title.$touch()"
                class="form-control"
              />
              <div v-if="$v.localMovie.title.$error && $v.localMovie.title.$dirty" class="text-danger">
                <span v-if="$v.localMovie.title.required">Title is required.</span>
                <span v-if="$v.localMovie.title.maxLength">Title must be less than 200 characters.</span>
              </div>
            </div>
            <div class="mb-3">
              <label for="director" class="form-label">Director:</label>
              <input
                type="text"
                v-model="$v.localMovie.director.$model"
                name="director"
                id="director"
                @blur="$v.localMovie.director.$touch()"
                class="form-control"
              />
              <div v-if="$v.localMovie.director.$error && $v.localMovie.director.$dirty" class="text-danger">
                <span v-if="$v.localMovie.director.required">Director is required.</span>
              </div>
            </div>
            <div class="mb-3">
              <label for="year" class="form-label">Year:</label>
              <input
                type="number"
                v-model="$v.localMovie.year.$model"
                name="year"
                id="year"
                @blur="$v.localMovie.year.$touch()"
                class="form-control"
              />
              <div v-if="$v.localMovie.year.$error && $v.localMovie.year.$dirty" class="text-danger">
                <span v-if="$v.localMovie.year.required">Year is required.</span>
                <span v-if="$v.localMovie.year.between">Year must be between 1900 and 2200.</span>
              </div>
            </div>
            <div class="mb-3">
              <label for="rate" class="form-label">Rating:</label>
              <input
                type="number"
                v-model="$v.localMovie.rate.$model"
                name="rate"
                id="rate"
                step="0.1"
                @blur="$v.localMovie.rate.$touch()"
                class="form-control"
              />
              <div v-if="$v.localMovie.rate.$error && $v.localMovie.rate.$dirty" class="text-danger">
                <span v-if="$v.localMovie.rate.required">Rating is required.</span>
                <span v-if="$v.localMovie.rate.$invalid">Rating must be between 0.0 and 10.0.</span>
              </div>
            </div>
            <p v-if="errorMessage" class="text-danger">{{ errorMessage }}</p>
            <div class="modal-footer">
              <button type="submit" class="btn" :class="isEditing ? 'btn-warning' : 'btn-success'" :disabled="$v.$invalid">
                {{ isEditing ? 'Update' : 'Add' }}
              </button>
              <button type="button" class="btn btn-secondary" @click="closeModal">Cancel</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import useVuelidate from "@vuelidate/core";
import { ref, watch } from "vue";
import { required, maxLength, between, minValue, maxValue } from "@vuelidate/validators";
import axios from "axios";

export default {
  name: "MovieModalComponent",
  props: {
    isEditing: Boolean,
    movie: Object,
  },
  setup(props, { emit }) {
    const localMovie = ref({ ...props.movie });
    const errorMessage = ref("");

    const rules = {
      localMovie: {
        title: { required, maxLength: maxLength(200) },
        director: { required },
        year: { required, between: between(1900, 2200) },
        rate: { required, minValue: minValue(0.0), maxValue: maxValue(10.0) },
      },
    };

    const $v = useVuelidate(rules, { localMovie });

    watch(
      () => props.movie,
      (newValue) => {
        localMovie.value = { ...newValue };
      },
      { deep: true, immediate: true }
    );

    const onSave = async () => {
      if ($v.value.$invalid) {
        $v.value.$touch();
        return;
      }

      try {
        const response = await axios.get(`http://localhost:5003/api/movie`);
        const existingMovies = response.data;
        const duplicateMovie = existingMovies.find(
          (movie) =>
            movie.title === localMovie.value.title &&
            movie.director === localMovie.value.director &&
            movie.id !== localMovie.value.id
        );
        if (duplicateMovie) {
          errorMessage.value = "A movie with the same title and director already exists.";
          return;
        }
        emit("save-movie", localMovie.value);
      } catch (error) {
        if (error.response && error.response.status === 409) {
          errorMessage.value = "A movie with the same title and director already exists.";
        } else {
          errorMessage.value = "An error occurred while saving the movie.";
        }
      }
    };

    const closeModal = () => {
      emit("close-modal");
    };

    return {
      localMovie,
      $v,
      onSave,
      closeModal,
      errorMessage,
    };
  },
};
</script>

<style scoped>
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}
.modal-content {
  background: white;
  padding: 20px;
  border-radius: 4px;
  width: 400px;
}
.btn-success {
  background-color: #10c228;
  color: white;
}
.btn-success:hover {
  background-color: hsl(120, 2%, 37%);
  color: white;
}
.btn-warning {
  background-color: #ffc107;
  color: black;
}
.btn-warning:hover {
  background-color: #e0a800;
  color: black;
}
</style>
