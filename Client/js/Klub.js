document.addEventListener("DOMContentLoaded", function () {
  const klubList = document.getElementById("klubList");
  const createForm = document.getElementById("createForm");
  const updateForm = document.getElementById("updateForm");
  const deleteForm = document.getElementById("deleteForm");
  const teamForm = document.getElementById("teamForm");
  populateTims();

  async function displayKlubList() {
    klubList.innerHTML = "";
    const klubovi = await getKlubovi();

    klubovi.forEach((klub) => {
      const li = document.createElement("li");
      li.textContent = `${klub.naziv} - ${klub.adresa} - ${klub.godinaOsnivanja}`;
      klubList.appendChild(li);
    });
  }

  async function getKlubovi() {
    const response = await fetch("http://localhost:5064/api/Klub/get-all-klub");
    const data = await response.json();
    return data;
  }

  createForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(createForm);
    const klubData = {
      KlubId: formData.get("klubId"),
      Naziv: formData.get("naziv"),
      Adresa: formData.get("adresa"),
      GodinaOsnivanja: formData.get("godinaOsnivanja"),
      BrojTelefona: formData.get("brojTelefona"),
      Email: formData.get("email"),
    };
    await createKlub(klubData);
    await displayKlubList();
    createForm.reset();
  });

  async function createKlub(klubData) {
    const response = await fetch("http://localhost:5064/api/Klub/create-klub", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(klubData),
    });
    const data = await response.json();
    console.log("Klub created:", data);
  }

  async function fetchKlubDetails() {
    const klubName = document.getElementById("updateNaziv").value;
    const klub = await getKlubByName(klubName);

    if (klub) {
      document.getElementById("updateAdresa").value = klub.adresa || "";
      document.getElementById("updateBrojTelefona").value =
        klub.brojTelefona || "";
      document.getElementById("updateEmail").value = klub.email || "";
    } else {
      console.error(`Klub with name ${klubName} not found.`);
    }
  }

  document
    .getElementById("fetchKlub")
    .addEventListener("click", fetchKlubDetails);

  updateForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(updateForm);
    const klubName = document.getElementById("updateNaziv").value;

    const klub = await getKlubByName(klubName);

    if (!klub) {
      console.error(`Klub with name ${klubName} not found.`);
      return;
    }

    const updatedData = {
      Adresa: document.getElementById("updateAdresa").value,
      BrojTelefona: document.getElementById("updateBrojTelefona").value,
      Email: document.getElementById("updateEmail").value,
    };

    await updateKlub(klub.naziv, updatedData);
    await displayKlubList();
    updateForm.reset();
  });

  async function updateKlub(name, updatedData) {
    const klub = await getKlubByName(name);
    if (!klub) {
      console.error(`Klub with name ${name} not found.`);
      return;
    }
    const updatedKlubData = { ...klub, ...updatedData };
    const response = await fetch(
      `http://localhost:5064/api/Klub/put-klub/${klub.klubId}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(updatedKlubData),
      }
    );
    const data = await response.json();
    console.log("Klub updated:", data);
  }

  async function getKlubByName(name) {
    const response = await fetch(
      `http://localhost:5064/api/Klub/get-klub-by-name/${name}`
    );
    if (response.ok) {
      return await response.json();
    } else {
      console.error(`Failed to get Klub by name ${name}.`);
      return null;
    }
  }

  deleteForm.addEventListener("submit", async function (event) {
    event.preventDefault();
    const klubName = document.getElementById("deleteNaziv").value;

    const klub = await getKlubByName(klubName);

    if (!klub) {
      console.error(`Klub with name ${klubName} not found.`);
      return;
    }

    await deleteKlub(klub.klubId);
    await displayKlubList();
    deleteForm.reset();
  });

  async function deleteKlub(id) {
    const response = await fetch(
      `http://localhost:5064/api/Klub/delete-klub/${id}`,
      {
        method: "DELETE",
      }
    );

    if (response.ok) {
      console.log(`Klub with ID ${id} deleted successfully.`);
    } else {
      console.error(`Failed to delete Klub with ID ${id}.`);
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
    const klubData = {
      Naziv: formData.get("naziv"),
    };
    const klub = await getKlubByName(klubData.Naziv);
    console.log(klub);
    const selectedTeams = Array.from(
      document.getElementById("teams").selectedOptions
    ).map((option) => option.value);

    fetch(
      `http://localhost:5064/api/Klub/dodaj-tim-klubu/${
        klub.klubId
      }/${selectedTeams.join(",")}`,
      {
        method: "POST",
      }
    ).then((response) => {
      if (response.ok) {
        console.log("Team added to the club successfully");
      } else {
        console.error("Failed to add team to the club");
      }
    });
  });

  displayKlubList();
});
