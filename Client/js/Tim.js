document.addEventListener("DOMContentLoaded", function () {
  const timList = document.getElementById("timList");
  const createForm = document.getElementById("createForm");
  const updateForm = document.getElementById("updateForm");
  const deleteForm = document.getElementById("deleteForm");
  const sportSelect = document.getElementById("sportSelect");
  const trenerSelect = document.getElementById("trenerSelect");
  populateSportsDropdowns(sportSelect);
  populateCoatchesDropdowns(trenerSelect);
  populateCompatitors();

  createForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(createForm);
    const timId = Date.now();
    const timData = {
      Id: timId.toString(),
      Naziv: formData.get("naziv"),
    };
    await createTim(timData);
    createForm.reset();
  });

  async function createTim(timData) {
    const response = await fetch("http://localhost:5064/api/Tim/create-tim", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(timData),
    });
    const data = await response.json();
    console.log("Tim created:", data);
  }

  const btnSport = document.getElementById("submitSport");
  btnSport.addEventListener("click", async function (event) {
    console.log("clk");
    event.preventDefault();
    const formData = new FormData(updateForm);
    const timData = {
      Naziv: formData.get("naziv"),
    };
    await updateTimSport(timData);
    createForm.reset();
  });

  const btnTrener = document.getElementById("submitTrener");
  btnTrener.addEventListener("click", async function (event) {
    event.preventDefault();
    const formData = new FormData(updateForm);
    const timData = {
      Naziv: formData.get("naziv"),
    };
    await updateTimTrener(timData);
    createForm.reset();
  });

  async function updateTimSport(timData) {
    const tim = await getTimByName(timData.Naziv);
    console.log(tim);
    const selectedSportId = document.getElementById("sportSelect").value;
    const response2 = await fetch(
      `http://localhost:5064/api/Tim/dodaj-sport-timu/${tim.id}/${selectedSportId}`,
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

  async function updateTimTrener(timData) {
    const tim = await getTimByName(timData.Naziv);
    const selectedTrenerId = document.getElementById("trenerSelect").value;
    const response3 = await fetch(
      `http://localhost:5064/api/Trener/dodaj-tim-treneru/${selectedTrenerId}/${tim.id}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    const data3 = await response3;
    console.log(data3);
  }

  async function getTimById(id) {
    const response = await fetch(`http://localhost:5064/api/Tim/get-tim/${id}`);
    if (response.ok) {
      return await response.json();
    } else {
      console.error(`Failed to get Tim by ID ${id}.`);
      return null;
    }
  }

  deleteForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const timName = document.getElementById("deleteTimName").value;

    const tim = await getTimByName(timName);

    if (!tim) {
      console.error(`Tim with name ${timName} not found.`);
      return;
    }

    await deleteTim(tim.id);
    deleteForm.reset();
  });

  async function deleteTim(id) {
    const response = await fetch(
      `http://localhost:5064/api/Tim/delete-tim/${id}`,
      {
        method: "DELETE",
      }
    );

    if (response.ok) {
      console.log(`Tim with ID ${id} deleted successfully.`);
    } else {
      console.error(`Failed to delete Tim with ID ${id}.`);
    }
  }

  async function getTimByName(name) {
    const response = await fetch(
      `http://localhost:5064/api/Tim/get-tim-by-name/${name}`
    );
    if (response.ok) {
      return await response.json();
    } else {
      console.error(`Failed to get Tim by name ${name}.`);
      return null;
    }
  }

  async function populateSportsDropdowns(sportSelect) {
    const sports = await getSportovi();
    sports.forEach((sport) => {
      const option = document.createElement("option");
      option.value = sport.sportId;
      option.text = sport.naziv;
      sportSelect.appendChild(option);
    });
  }

  async function populateCoatchesDropdowns(trenerSelect) {
    const treneri = await getTreneri();
    treneri.forEach((trener) => {
      const option = document.createElement("option");
      option.value = trener.jmbg;
      option.text = trener.ime + " " + trener.prezime;
      trenerSelect.appendChild(option);
    });
  }

  async function getSportovi() {
    const response = await fetch(
      "http://localhost:5064/api/Sport/get-all-sportovi"
    );
    const data = await response.json();
    return data;
  }

  async function getTreneri() {
    const response = await fetch(
      "http://localhost:5064/api/Trener/get-all-trener"
    );
    const data = await response.json();
    return data;
  }

  async function getSportById(sportId) {
    try {
      const response = await fetch(
        `http://localhost:5064/api/Sport/get-sport/${sportId}`
      );

      if (!response.ok) {
        throw new Error(`Failed to fetch sport with ID ${sportId}`);
      }

      const sport = await response.json();
      return sport;
    } catch (error) {
      console.error("Error fetching sport:", error.message);
      return null;
    }
  }

  async function populateCompatitors() {
    fetch("http://localhost:5064/api/Takmicar/get-all-takmicar")
      .then((response) => response.json())
      .then((data) => {
        const competitorsSelect = document.getElementById("competitors");

        data.forEach((competitor) => {
          const option = document.createElement("option");
          option.value = competitor.jmbg;
          option.text = `${competitor.ime} ${competitor.prezime}`;
          competitorsSelect.appendChild(option);
        });
      });
  }

  const btnTakmicar = document.getElementById("submitTakmicar");
  btnTakmicar.addEventListener("click", async function (event) {
    console.log("clk");
    event.preventDefault();
    const formData = new FormData(updateForm);
    const timData = {
      Naziv: formData.get("naziv"),
    };
    await updateTimTakmicar(timData);
    createForm.reset();
  });

  async function updateTimTakmicar(timData) {
    const tim = await getTimByName(timData.Naziv);

    const selectedCompetitors = Array.from(
      document.getElementById("competitors").selectedOptions
    ).map((option) => option.value);

    fetch(
      `http://localhost:5064/api/Tim/dodaj-takmicara-timu/${
        tim.id
      }/${selectedCompetitors.join(",")}`,
      {
        method: "POST",
      }
    ).then((response) => {
      if (response.ok) {
        console.log("Competitors added to the team successfully");
      } else {
        console.error("Failed to add competitors to the team");
      }
    });
  }
});
