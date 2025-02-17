document.addEventListener("DOMContentLoaded", function () {
  const createForm = document.getElementById("createForm");
  const deleteForm = document.getElementById("deleteForm");
  const teamForm = document.getElementById("teamForm");
  const sportForm = document.getElementById("sportForm");
  const sportSelect = document.getElementById("sportSelect");
  populateTims();
  populateSportsDropdowns(sportSelect);

  function getParametarIzUrla(parametar) {
    const urlSearchParams = new URLSearchParams(window.location.search);
    return urlSearchParams.get(parametar);
  }

  const takmicenjeId = getParametarIzUrla("takmicenjeId");
  console.log(takmicenjeId);

  createForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const utakmicaId = Date.now();
    const formData = new FormData(createForm);
    const utakmicaData = {
      UtakmicaId: utakmicaId.toString(),
      Naziv: formData.get("naziv"),
      Datum: formData.get("datum"),
      Kolo: formData.get("kolo"),
    };
    await createUtakmica(utakmicaData);
    createForm.reset();
  });

  async function createUtakmica(utakmicaData) {
    const response = await fetch(
      "http://localhost:5064/api/Utakmica/create-utakmica",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(utakmicaData),
      }
    );
    const data = await response.json();
    console.log("Utakmica created:", data);

    await dodajTakmicenjeUtakmici(utakmicaData.UtakmicaId, takmicenjeId);
    console.log(utakmicaData.UtakmicaId, takmicenjeId);
  }

  async function dodajTakmicenjeUtakmici(idUtakmice, idTakmicenja) {
    const response = await fetch(
      `http://localhost:5064/api/Utakmica/dodaj-takmicenje-utakmici/${idUtakmice}/${idTakmicenja}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
  }

  async function getUtakmicaByName(name) {
    const response = await fetch(
      `http://localhost:5064/api/Utakmica/get-utakmica-by-name/${name}`
    );
    if (response.ok) {
      return await response.json();
    } else {
      console.error(`Failed to get Utakmica by name ${name}.`);
      return null;
    }
  }

  deleteForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(deleteForm);
    const utakmicaId = formData.get("deleteUtakmicaId");
    await deleteUtakmica(utakmicaId);
    deleteForm.reset();
  });

  async function deleteUtakmica(utakmicaId) {
    const response = await fetch(
      `http://localhost:5064/api/Utakmica/delete-utakmica/${utakmicaId}`,
      {
        method: "DELETE",
      }
    );

    if (response.ok) {
      console.log(`Utakmica with ID ${utakmicaId} deleted successfully.`);
    } else {
      console.error(`Failed to delete Utakmica with ID ${utakmicaId}.`);
    }
  }

  async function populateTims() {
    fetch("http://localhost:5064/api/Tim/get-all-teams")
      .then((response) => response.json())
      .then((data) => {
        const teamsSelect = document.getElementById("teams");

        data.forEach((team) => {
          const option = document.createElement("option");
          option.value = team.id;
          option.text = `${team.naziv}`;
          teamsSelect.appendChild(option);
        });
      });
  }

  teamForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(teamForm);
    const utakmicaData = {
      Naziv: formData.get("naziv"),
    };
    const utakmica = await getUtakmicaByName(utakmicaData.Naziv);
    console.log(utakmica);
    const selectedTeams = Array.from(
      document.getElementById("teams").selectedOptions
    ).map((option) => option.value);

    fetch(
      `http://localhost:5064/api/Utakmica/dodaj-timove-utakmici/${
        utakmica.utakmicaId
      }/${selectedTeams.join(",")}`,
      {
        method: "POST",
      }
    ).then((response) => {
      if (response.ok) {
        console.log("Team added to the match successfully");
      } else {
        console.error("Failed to add team to the match");
      }
    });
  });

  async function populateSportsDropdowns(sportSelect) {
    const sports = await getSportovi();
    sports.forEach((sport) => {
      const option = document.createElement("option");
      option.value = sport.sportId;
      option.text = sport.naziv;
      sportSelect.appendChild(option);
    });
  }

  async function getSportovi() {
    const response = await fetch(
      "http://localhost:5064/api/Sport/get-all-sportovi"
    );
    const data = await response.json();
    return data;
  }

  const btnSport = document.getElementById("submitSport");
  btnSport.addEventListener("click", async function (event) {
    event.preventDefault();
    const formData = new FormData(sportForm);
    const utakmicaData = {
      Naziv: formData.get("naziv"),
    };
    await updateTimSport(utakmicaData);
    createForm.reset();
  });

  async function updateTimSport(utakmicaData) {
    const utakmica = await getUtakmicaByName(utakmicaData.Naziv);
    console.log(utakmica);
    const selectedSportId = document.getElementById("sportSelect").value;
    const response2 = await fetch(
      `http://localhost:5064/api/Utakmica/dodaj-sport-utakmici/${utakmica.utakmicaId}/${selectedSportId}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    const data2 = await response2;
    console.log(data2);
  }
});
