document.addEventListener("DOMContentLoaded", function () {
  const takmicenjeList = document.getElementById("takmicenjaList");
  const createForm = document.getElementById("createForm");
  const updateForm = document.getElementById("updateForm");
  const deleteForm = document.getElementById("deleteForm");
  const utakmicaForm = document.getElementById("utakmicaForm");

  async function displayTakmicenjeList() {
    takmicenjeList.innerHTML = "";
    const takmicenja = await getTakmicenja();

    takmicenja.forEach(async (takmicenje) => {
      const li = document.createElement("li");
      li.textContent = `${takmicenje.naziv} - ${takmicenje.mestoOdrzavanja} - ${takmicenje.datumOd} - ${takmicenje.datumDo}`;

      const btnDodajUtakmicu = document.createElement("button");
      btnDodajUtakmicu.textContent = "Dodaj utakmicu";
      btnDodajUtakmicu.addEventListener("click", () => {
        const takmicenjeId = takmicenje.takmicenjeId;
        const novaStranicaURL = `Utakmica.html?takmicenjeId=${takmicenjeId}`;
        window.location.href = novaStranicaURL;

        console.log(`Kliknuto na dugme za takmičenje: ${takmicenjeId}`);
      });

      let utakmiceList;

      const btnPrikaziUtakmice = document.createElement("button");
      btnPrikaziUtakmice.textContent = "Prikaži utakmice";
      btnPrikaziUtakmice.addEventListener("click", async () => {
        const nazivTakmicenja = takmicenje.naziv;
        if (utakmiceList) {
          console.log("Utakmice su već učitane.");
          return;
        }

        try {
          const response = await fetch(
            `http://localhost:5064/api/Takmicenje/get-utakmice-by-takmicenje/${nazivTakmicenja}`
          );

          if (!response.ok) {
            throw new Error(
              `Error fetching utakmice for takmicenje ${nazivTakmicenja}`
            );
          }

          const data = await response.json();
          utakmiceList = document.createElement("ul");

          data.forEach(async (utakmica) => {
            const utakmicaItem = document.createElement("li");
            console.log(utakmica);
            const response = await fetch(
              `http://localhost:5064/api/Utakmica/teamNames/${utakmica.utakmicaId}`
            );

            if (!response.ok) {
              throw new Error("Greška prilikom preuzimanja timova");
            }
            const timovi = await response.json();
            console.log(timovi);
            const timoviString =
              timovi.length > 0
                ? timovi.map((tim) => tim.naziv).join(" vs ")
                : "Nema dostupnih timova";
            utakmicaItem.textContent = `Utakmica: ${utakmica.naziv} -Timovi: ${timoviString} - "Kolo:"- ${utakmica.kolo}`;

            const razmak = document.createElement("br");
            utakmicaItem.appendChild(razmak);
            timovi.forEach((tim, index) => {
              const poeniInput = document.createElement("input");
              poeniInput.id = `poeni_${index}`;
              poeniInput.placeholder = `Poeni za ${tim.naziv}`;
              utakmicaItem.appendChild(poeniInput);
            });

            const button3 = document.createElement("button");
            button3.textContent = "Dodaj rezultat";
            button3.addEventListener("click", async () => {
              dodajRezultat(takmicenje.takmicenjeId, utakmica.utakmicaId);
            });

            utakmiceList.appendChild(utakmicaItem);
            utakmiceList.appendChild(button3);
          });

          li.appendChild(utakmiceList);
        } catch (error) {
          console.error(error);
        }
      });

      const btnPrikaziLeaderboard = document.createElement("button");
      btnPrikaziLeaderboard.textContent = "Prikaži leaderboard";
      btnPrikaziLeaderboard.addEventListener("click", async () => {
        const novaStranicaURL = `RezultatTakmicenja.html?takmicenjeId=${takmicenje.takmicenjeId}`;
        window.location.href = novaStranicaURL;
      });

      li.appendChild(btnDodajUtakmicu);
      li.appendChild(btnPrikaziUtakmice);
      li.appendChild(btnPrikaziLeaderboard);
      takmicenjeList.appendChild(li);
    });
  }

  async function getTakmicenja() {
    const response = await fetch(
      "http://localhost:5064/api/Takmicenje/get-all-takmicenja"
    );
    const data = await response.json();
    return data;
  }

  createForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(createForm);
    const takmicenjeId = Date.now();
    const takmicenjeData = {
      TakmicenjeId: takmicenjeId.toString(),
      MestoOdrzavanja: formData.get("mestoOdrzavanja"),
      Naziv: formData.get("naziv"),
      DatumOd: formData.get("datumOd"),
      DatumDo: formData.get("datumDo"),
    };
    await createTakmicenje(takmicenjeData);
    await displayTakmicenjeList();
    createForm.reset();
  });

  async function createTakmicenje(takmicenjeData) {
    const response = await fetch(
      "http://localhost:5064/api/Takmicenje/create-takmicenje",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(takmicenjeData),
      }
    );
    const data = await response.json();
    console.log("Takmicenje created:", data);
  }

  async function fetchTakmicenjeDetails() {
    const takmicenjeName = document.getElementById("updateNaziv").value;
    const takmicenje = await getTakmicenjeByName(takmicenjeName);

    if (takmicenje) {
      document.getElementById("updateMesto").value =
        takmicenje.mestoOdrzavanja || "";
      document.getElementById("updateDatumOd").value = takmicenje.datumOd || "";
      document.getElementById("updateDatumDo").value = takmicenje.datumDo || "";
    } else {
      console.error(`Takmicenje with name ${takmicenjeName} not found.`);
    }
  }

  document
    .getElementById("fetchTakmicenje")
    .addEventListener("click", fetchTakmicenjeDetails);

  updateForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(updateForm);
    const takmicenjeName = document.getElementById("updateNaziv").value;

    const takmicenje = await getTakmicenjeByName(takmicenjeName);

    if (!takmicenje) {
      console.error(`Takmicenje with name ${takmicenjeName} not found.`);
      return;
    }

    const updatedData = {
      MestoOdrzavanja: document.getElementById("updateMesto").value,
      DatumOd: document.getElementById("updateDatumOd").value,
      DatumDo: document.getElementById("updateDatumDo").value,
    };
    console.log(`${updatedData}aaaaaaaaaaaaa`);
    await updateTakmicenje(takmicenje.naziv, updatedData);

    await displayTakmicenjeList();
    updateForm.reset();
  });

  async function updateTakmicenje(name, updatedData) {
    const takmicenje = await getTakmicenjeByName(name);
    if (!takmicenje) {
      console.error(`Takmicenje with name ${name} not found.`);
      return;
    }
    const updatedTakmicenjeData = { ...takmicenje, ...updatedData };
    const response = await fetch(
      `http://localhost:5064/api/Takmicenje/update-takmicenje/${takmicenje.takmicenjeId}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(updatedTakmicenjeData),
      }
    );
    const data = await response.json();
    console.log("Takmicenje updated:", data);
  }

  async function getTakmicenjeByName(name) {
    const response = await fetch(
      `http://localhost:5064/api/Takmicenje/get-takmicenje-by-name/${name}`
    );
    if (response.ok) {
      return await response.json();
    } else {
      console.error(`Failed to get Takmicenje by name ${name}.`);
      return null;
    }
  }

  deleteForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const takmicenjeName = document.getElementById("deleteTakmicenjeId").value;

    const takmicenje = await getTakmicenjeByName(takmicenjeName);

    if (!takmicenje) {
      console.error(`Takmicenje with name ${takmicenjeName} not found.`);
      return;
    }

    await deleteTakmicenje(takmicenje.takmicenjeId);
    await displayTakmicenjeList();
    deleteForm.reset();
  });

  async function deleteTakmicenje(id) {
    const response = await fetch(
      `http://localhost:5064/api/Takmicenje/delete-takmicenje/${id}`,
      {
        method: "DELETE",
      }
    );

    if (response.ok) {
      console.log(`Takmicenje with ID ${id} deleted successfully.`);
    } else {
      console.error(`Failed to delete Takmicenje with ID ${id}.`);
    }
  }

  utakmicaForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const takmicenjeName = document.getElementById("naziv").value;
    console.log(takmicenjeName);
    populateTims(takmicenjeName);
  });

  async function populateTims(nazivTakmicenja) {
    var selectElemet = document.getElementById("utakmice");
    selectElemet.innerHTML = "";
    fetch(
      `http://localhost:5064/api/Takmicenje/get-utakmice-by-takmicenje/${nazivTakmicenja}`
    )
      .then((response) => response.json())
      .then((data) => {
        const utakmicaSelect = document.getElementById("utakmice");

        data.forEach((utakmica) => {
          const option = document.createElement("option");
          option.value = utakmica.utakmicaId;
          option.text = `${utakmica.naziv}`;
          utakmicaSelect.appendChild(option);

          option.addEventListener("dblclick", function () {
            prikaziRezultate(option.value);
          });
        });
      });
  }
  async function prikaziRezultate(idUtakmice) {
    const rezultatResponse = await fetch(
      `http://localhost:5064/api/Utakmica/imaRezultat/${idUtakmice}`
    );
    const utakmicaImaRezultat = await rezultatResponse.json();
    if (!utakmicaImaRezultat) {
      console.log("Utakmica jos uvek nije odigrana.");
      renderLeaderboard("Utakmica jos uvek nije odigrana.");
      return;
    }
    var leaderboard = document.createElement("ul");
    leaderboard.id = "leaderboard";

    const timIdsResponse = await fetch(
      `http://localhost:5064/api/Utakmica/teamIds/${idUtakmice}`
    );
    const timIds = await timIdsResponse.json();

    for (const timId of timIds) {
      await fetch(
        `http://localhost:5064/api/LeaderboardUtakmica/${idUtakmice}/${timId}`,
        {
          method: "POST",
        }
      );
    }

    fetch(`http://localhost:5064/api/LeaderboardUtakmica/${idUtakmice}`)
      .then((response) => response.json())
      .then((data) => {
        renderLeaderboard(data);
      })
      .catch((error) => console.error("Error fetching leaderboard:", error));

    async function renderLeaderboard(leaderboardData) {
      console.log(leaderboardData);
      const leaderboardElement = document.getElementById("leaderboard");
      if (leaderboardData == "Utakmica jos uvek nije odigrana.") {
        leaderboardElement.innerHTML = "Utakmica jos uvek nije odigrana.";
        return;
      }

      leaderboardElement.innerHTML = "";

      const teamNames = {};
      await Promise.all(
        leaderboardData.map(async (entry) => {
          const nazivTima = await getTimById(entry.timId);
          teamNames[entry.timId] = nazivTima;
        })
      );

      const table = document.createElement("table");
      table.classList.add("leaderboard-table");

      const headerRow = table.insertRow(0);
      const headerCell1 = headerRow.insertCell(0);
      const headerCell2 = headerRow.insertCell(1);
      headerCell1.textContent = "Naziv tima";
      headerCell2.textContent = "Broj poena";

      for (const entry of leaderboardData) {
        const row = table.insertRow(-1);
        const nazivTima = teamNames[entry.timId] || "N/A";
        const cell1 = row.insertCell(0);
        const cell2 = row.insertCell(1);
        cell1.textContent = nazivTima;
        cell2.textContent = entry.poeni;
      }

      leaderboardElement.appendChild(table);
    }

    utakmicaForm.appendChild(leaderboard);
  }

  async function getTimById(id) {
    const response = await fetch(`http://localhost:5064/api/Tim/get-tim/${id}`);
    if (response.ok) {
      const timInfo = await response.json();

      return timInfo.naziv || null;
    } else {
      console.error(`Failed to get Tim by ID ${id}.`);
      return null;
    }
  }

  async function dodajRezultat(takmicenjeId, utakmicaId) {
    try {
      const rezultatResponse = await fetch(
        `http://localhost:5064/api/Utakmica/imaRezultat/${utakmicaId}`
      );
      const utakmicaImaRezultat = await rezultatResponse.json();
      if (utakmicaImaRezultat) {
        console.log("Utakmica vec ima rezultat, ne moze se dodati novi.");
        return;
      }
      const timIdsResponse = await fetch(
        `http://localhost:5064/api/Utakmica/teamIds/${utakmicaId}`
      );
      const timIds = await timIdsResponse.json();
      for (let i = 0; i < timIds.length; i++) {
        const poeni = document.getElementById(`poeni_${i}`).value;
        await fetch(
          `http://localhost:5064/api/Rezultat/createAndAssignToTim/${utakmicaId}/${timIds[i]}/${poeni}`,
          {
            method: "POST",
          }
        );
      }

      for (const timId of timIds) {
        await fetch(
          `http://localhost:5064/api/LeaderboardUtakmica/${utakmicaId}/${timId}`,
          {
            method: "POST",
          }
        );
        console.log(`Success for match ${utakmicaId}`);
      }
      console.log("Rezultati dodati");
    } catch (error) {
      console.error(error);
    }
  }

  displayTakmicenjeList();
});
