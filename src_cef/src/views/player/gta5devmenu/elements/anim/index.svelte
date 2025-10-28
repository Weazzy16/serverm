<script>
    import './main.css';
    import './circle.css';
    import Animations from 'json/animations.js';
    import AnimElement from './element.svelte';
    import { storeAnimFavorites } from 'store/animation';
    import { spring } from 'svelte/motion';
    import { executeClient } from 'api/rage';
    export let selectView;
    export let searchText;

    // Menu items
    let animMenuList = [
        { id: 1, title: 'Действия', count: 0 },
        { id: 2, title: 'Позы', count: 0 },
        { id: 3, title: 'Позитивные', count: 0 },
        { id: 4, title: 'Негативные', count: 0 },
        { id: 5, title: 'Танцы', count: 0 },
        { id: 6, title: 'Прочие', count: 0 },
        { id: 7, title: 'Эксклюзивные', count: 0 },
        { id: 8, title: 'Избраное', count: 0 },
        { id: 9, title: 'Круговое меню', count: 0 }
    ];
    let onSelectedView1;
$: onSelectedView1 = animMenuList.find(x => x.id === +selectView.replace('Anim',''));

    // Count animations
    Object.values(Animations).forEach(anim => {
        animMenuList.forEach(item => {
            if (anim[0] === item.title) item.count++;
        });
    });

    // Favorites
    let favoritesAnim = [];
    storeAnimFavorites.subscribe(val => {
        favoritesAnim = val;
        animMenuList[8].count = favoritesAnim.length;
    });

    // Pagination
    const itemsPerPage = 25;
    let page = 1;

    // Scroll handler
    function handleScroll(e) {
        const el = e.target;
        if (el.scrollTop + el.clientHeight >= el.scrollHeight - 5) {
            page += 1;
        }
    }

    // Drag/drop logic
    let DragonDropData = '';
    let coords = spring({ x:0, y:0 }, { stiffness:1, damping:1 });
    const handleMouseDown = (e, item) => {
        DragonDropData = item;
        coords.set({ x: e.clientX, y: e.clientY });
    };

    let favoriteIndex = -1;
    function handleFavoriteSlotMouseEnter(index) {
        favoriteIndex = index;
    }
    function handleFavoriteSlotMouseLeave() {
        favoriteIndex = -1;
    }
    const handleGlobalMouseMove = e => {
        if (DragonDropData) coords.set({ x: e.clientX, y: e.clientY });
    };
    const handleGlobalMouseUp = () => DragonDropData = '';

    // Play animation
    const onPlayAnimation = item => executeClient('client.animation.play', item);

    // Favorites helpers
    const IsFavorite = (index, AnimListFavorites) => AnimListFavorites?.includes(`${onSelectedView1.id}_${index}`);

    const AddFavorite = (event, item) => {
        event.stopPropagation();
        window.animationStore.addAnimFavorite(item);
    };

    const DellFavorite = (event, item) => {
        event.stopPropagation();
        window.animationStore.dellAnimFavorite(item);
    };

    let enterAnim = "";

    function handleSlotMouseEnter(index) {
        if (DragonDropData) return;
        enterAnim = `${onSelectedView1.id}_${index}`;
    }
    function handleSlotMouseLeave() {
        enterAnim = "";
    }

    // Circle Menu logic
     let circleMenu = Array(8).fill(null);
    let selectedCircleSlot = null;
    let rotateDegree = 45;

    function selectCircleSlot(index) {
        selectedCircleSlot = index;
        selectView = 'Anim8';
    }

    function assignAnimationToCircle(animation) {
    if (selectedCircleSlot !== null) {
        // Делаем новый массив
        circleMenu = circleMenu.map((item, idx) =>
            idx === selectedCircleSlot ? animation : item
        );
        selectedCircleSlot = null;
        selectView = 'Anim9'; // Возвращаемся на круговое меню
    }
}


</script>
<style>

    
</style>
<svelte:window on:mousemove={handleGlobalMouseMove} on:mouseup={handleGlobalMouseUp} />

<div class="animations full-width full-height">
          {#if onSelectedView1.id === 9}
        <div class="circle-menu full-height full-width">
            <div class="circle" >
                <div class="strokes full-width full-height">
                    {#each circleMenu as slot, index}
                        <div class="stroke-wrapper justify-center align-start"
                            style="transform: rotate({rotateDegree * index}deg);">
                            <svg xmlns="http://www.w3.org/2000/svg"  viewBox="0 0 304.29 38.96" class="stroke-wrapper__picture">
  <path d="M149.41,40.74A387.51,387.51,0,0,1,300.66,70.18l3.63-8.79A398.68,398.68,0,0,0,0,61.39l3.64,8.78A389.14,389.14,0,0,1,149.41,40.74Z" transform="translate(0 -31.22)"/>
</svg>
                           
                        </div>
                    {/each}
                </div>
                {#each circleMenu as slot, index}
                    <div class="circle-item full-width full-height justify-center"
                        style="transform: rotate({rotateDegree * index}deg);">
                        <div class="tab selectable {slot ? '' : 'empty'}" on:click={() => selectCircleSlot(index)}>
                            
                            <svg class="tab__picture"xmlns="http://www.w3.org/2000/svg" width="261.612" height="162.258" viewBox="0 0 261.612 162.258">
        <path d="M790.312,285.9l56.67-136.8a348.829,348.829,0,0,0-261.612,0l56.657,136.8A200.468,200.468,0,0,1,790.312,285.9Z" transform="translate(-585.37 -123.64)"/>
    </svg>
                            {#if slot}
                                <span class="tab__icon">{Animations[slot][2]}</span>
                            {/if}
                        </div>
                    </div>
                {/each}
            </div>
        </div>
    {:else}
        <div class="anims-list full-width full-height row-block" style="overflow-y:auto;scroll-behavior:smooth;" on:scroll|passive={handleScroll}>
             {#if onSelectedView1.id === 8}
  {#each favoritesAnim.slice(0, itemsPerPage * page) as animation, index}
                    {#if !searchText || Animations[animation][2].toLowerCase().includes(searchText.toLowerCase().trim())}
                     {#if Animations[animation]}
                        <AnimElement
                            title={Animations[animation][2]}
                            isEnterAnim={favoriteIndex === index}
                            use={animation}
                            isFavorite={true}
                            dellFavorite={DellFavorite}
                            {onPlayAnimation}
                            on:mousedown={(e) => handleMouseDown(e, animation)}
                            on:mouseenter={() => handleFavoriteSlotMouseEnter(index)}
                            on:mouseleave={handleFavoriteSlotMouseLeave}
                            on:click={() => assignAnimationToCircle(animation)}
                        />
                    {/if}
                        {/if}

                {/each}
            {:else}
                {#each Object.values(Animations).filter(anim => anim[0] === onSelectedView1.title && (!searchText || anim[2].toLowerCase().includes(searchText.toLowerCase().trim()))).slice(0, itemsPerPage * page) as animation}
                    <AnimElement
                        title={animation[2]}
                        isEnterAnim={enterAnim === `${onSelectedView1.id}_${animation[1]}`}
                        use={`${onSelectedView1.id}_${animation[1]}`}
                        isFavorite={IsFavorite(animation[1], favoritesAnim)}
                        addFavorite={AddFavorite}
                        dellFavorite={DellFavorite}
                        {onPlayAnimation}
                        on:mousedown={(e) => handleMouseDown(e, `${onSelectedView1.id}_${animation[1]}`)}
                        on:mouseenter={() => handleSlotMouseEnter(animation[1])}
                        on:mouseleave={handleSlotMouseLeave} />
                {/each}
            {/if}
        </div>
    {/if}
</div>
