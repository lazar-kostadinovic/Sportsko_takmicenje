document.addEventListener("DOMContentLoaded", function () {
  const sportoviList = document.getElementById("sportoviList");
  const createForm = document.getElementById("createForm");
  const updateForm = document.getElementById("updateForm");
  const deleteForm = document.getElementById("deleteForm");

  async function displaySportoviList() {
    sportoviList.innerHTML = "";
    const sportovi = await getSportovi();

    sportovi.forEach((sport) => {
      const li = document.createElement("li");
      li.textContent = `${sport.naziv} - Broj igrača: ${sport.brojIgraca}`;
      sportoviList.appendChild(li);
    });
  }

  async function getSportovi() {
    const response = await fetch(
      "http://localhost:5064/api/Sport/get-all-sportovi"
    );
    const data = await response.json();
    return data;
  }

  createForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(createForm);
    const sportData = {
      SportId: formData.get("SportId"),
      Naziv: formData.get("naziv"),
      brojIgraca: formData.get("brIgraca"),
    };
    await createSport(sportData);
    await displaySportoviList();
    createForm.reset();
  });

  async function createSport(sportData) {
    const response = await fetch(
      "http://localhost:5064/api/Sport/create-sport",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(sportData),
      }
    );
    const data = await response.json();
    console.log("Sport created:", data);
  }

  async function fetchSportDetails() {
    const sportName = document.getElementById("updateNaziv").value;
    const sport = await getSportByName(sportName);

    if (sport) {
      document.getElementById("updateBroj").value = sport.brojIgraca || "";
    } else {
      console.error(`Sport with name ${sportName} not found.`);
    }
  }

  document
    .getElementById("fetchSport")
    .addEventListener("click", fetchSportDetails);

  updateForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(updateForm);
    const sportName = document.getElementById("updateNaziv").value;

    const sport = await getSportByName(sportName);

    if (!sport) {
      console.error(`Sport with name ${sportName} not found.`);
      return;
    }

    const updatedData = {
      BrIgraca: document.getElementById("updateBroj").value,
    };

    await updateSport(sport.naziv, updatedData);
    await displaySportoviList();
    updateForm.reset();
  });

  async function updateSport(name, updatedData) {
    const sport = await getSportByName(name);
    if (!sport) {
      console.error(`Sport with name ${name} not found.`);
      return;
    }
    const updatedSportData = { ...sport, ...updatedData };
    const response = await fetch(
      `http://localhost:5064/api/Sport/update-sport/${sport.sportId}/${updatedData.BrIgraca}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(updatedSportData),
      }
    );
    const data = await response.json();
    console.log("Sport updated:", data);
  }

  async function getSportByName(name) {
    const response = await fetch(
      `http://localhost:5064/api/Sport/get-sport-by-name/${name}`
    );
    if (response.ok) {
      return await response.json();
    } else {
      console.error(`Failed to get Sport by name ${name}.`);
      return null;
    }
  }

  deleteForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const sportName = document.getElementById("deleteSportId").value;

    const sport = await getSportByName(sportName);

    if (!sport) {
      console.error(`Sport with name ${sportName} not found.`);
      return;
    }

    await deleteSport(sport.sportId);
    await displaySportoviList();
    deleteForm.reset();
  });

  async function deleteSport(id) {
    const response = await fetch(
      `http://localhost:5064/api/Sport/delete-sport/${id}`,
      {
        method: "DELETE",
      }
    );

    if (response.ok) {
      console.log(`Sport with ID ${id} deleted successfully.`);
    } else {
      console.error(`Failed to delete Sport with ID ${id}.`);
    }
  }

  displaySportoviList();
});
