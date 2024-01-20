document.addEventListener("DOMContentLoaded", async function () {
  const listaTakmicara = document.getElementById("listTakmicara");
  const kreirajForma = document.getElementById("kreirajForma");
  const izmeniForma = document.getElementById("izmeniForma");
  const izbrisiForma = document.getElementById("izbrisiForma");
  const sportForma = document.getElementById("sportForma");
  const timForma = document.getElementById("timForma");
  const sportSelect = document.getElementById("sportSelect");
  const timSelect = document.getElementById("timSelect");
  displaySports();
  displayTakmicariList();
  displayTimList();

  async function displayTimList() {
    timSelect.innerHTML = "";
    const timovi = await getTimovi();

    timovi.forEach((tim) => {
      const option = document.createElement("option");
      option.value = tim.id;
      option.text = tim.naziv;
      timSelect.appendChild(option);
    });
  }

  async function getTimovi() {
    const response = await fetch("http://localhost:5064/api/Tim/get-all-teams");
    const data = await response.json();
    return data;
  }

  async function displaySports() {
    const sportovi = await getSportovi();
    sportovi.forEach((sport) => {
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

  async function displayTakmicariList() {
    listaTakmicara.innerHTML = "";
    const takmicari = await getTakmicari();

    const headerRow = document.createElement("tr");
    for (let key in takmicari[0]) {
      if (key !== "sport" && key !== "tim") {
        const th = document.createElement("th");
        th.innerHTML = key;
        headerRow.append(th);
      }
    }
    listaTakmicara.append(headerRow);

    takmicari.forEach((takmicar) => {
      const tr = document.createElement("tr");
      for (let key in takmicar) {
        if (key !== "tim" && key !== "sportovi") {
          const td = document.createElement("td");
          td.innerHTML = takmicar[key];
          tr.append(td);
        }
      }
      listaTakmicara.append(tr);
    });
  }
  async function getTakmicari() {
    const response = await fetch(
      "http://localhost:5064/api/Takmicar/get-all-takmicar"
    );
    const data = await response.json();
    return data;
  }

  async function kreairajTakmicara(takmicar) {
    const response = await fetch(
      "http://localhost:5064/api/Takmicar/create-takmicar",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(takmicar),
      }
    );
    if (response.status != 200) {
      window.alert("GRESKA!");
      return;
    }
    const data = await response.json();
    displayTakmicariList();
    window.alert("Takmicar dodat: " + JSON.stringify(data));
  }

  async function izmeniTakmicara(jmbg, adresa) {
    const response = await fetch(
      `http://localhost:5064/api/Takmicar/put-takmicar/${jmbg}/${adresa}`,
      {
        method: "PUT",
      }
    );
    if (response.status != 200) {
      window.alert(JSON.stringify(await response.json()));
      return;
    }
    const data = await response.json();
    displayTakmicariList();
    window.alert("Takmicar izmenjen: " + JSON.stringify(data));
  }

  async function izbrisiTakmicara(jmbg) {
    const response = await fetch(
      `http://localhost:5064/api/Takmicar/delete-takmicar/${jmbg}`,
      {
        method: "DELETE",
      }
    );
    if (response.status > 205) {
      window.alert(JSON.stringify(await response.json()));
      return;
    }
    displayTakmicariList();
  }

  async function dodajTakmicaraUSport(jmbg, sportId) {
    const response = await fetch(
      `http://localhost:5064/api/Takmicar/takmicar-trenira-sport/${jmbg}/${sportId}`,
      {
        method: "POST",
      }
    );
    if (response.status !== 200) {
      window.alert("GRESKA");
      return;
    }
    window.alert("Dodato!");
    displayTakmicariList();
  }

  async function dodajTakmicaraUTim(jmbg, timId) {
    const response = await fetch(
      `http://localhost:5064/api/Takmicar/dodaj-tim-takmicaru/${jmbg}/${timId}`,
      {
        method: "POST",
      }
    );
    if (response.status !== 200) {
      window.alert("GRESKA");
      return;
    }
    window.alert("Dodato!");
    displayTakmicariList();
  }

  kreirajForma.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(kreirajForma);
    const takmicar = {
      jmbg: formData.get("jmbg"),
      ime: formData.get("ime"),
      prezime: formData.get("prezime"),
      datumRodjenja: formData.get("datumRodjenja"),
      brojTelefona: formData.get("brojTelefona"),
      pol: formData.get("pol"),
      adresa: formData.get("adresa"),
      drzava: formData.get("drzava"),
    };
    kreairajTakmicara(takmicar);
  });

  izmeniForma.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(izmeniForma);
    izmeniTakmicara(formData.get("jmbg"), formData.get("adresa"));
  });

  izbrisiForma.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(izbrisiForma);
    izbrisiTakmicara(formData.get("jmbg"));
  });

  sportForma.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(sportForma);
    dodajTakmicaraUSport(formData.get("jmbg"), formData.get("sportId"));
  });

  timForma.addEventListener("submit", async function (event) {
    event.preventDefault();
    const formData = new FormData(timForma);
    dodajTakmicaraUTim(formData.get("jmbg"), formData.get("timId"));
  });
});
